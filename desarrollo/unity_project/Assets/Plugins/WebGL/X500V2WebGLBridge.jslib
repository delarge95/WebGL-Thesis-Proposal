function X500V2ResolveParentOrigin() {
  var targetOrigin = "*";
  if (typeof document !== "undefined" && document.referrer) {
    try {
      var parentUrl = new URL(document.referrer, window.location.href);
      if (parentUrl.origin && parentUrl.origin !== "null") {
        targetOrigin = parentUrl.origin;
      }
    } catch (ignoredReferrerError) {}
  } else if (
    typeof window !== "undefined" &&
    window.location &&
    window.location.origin &&
    window.location.origin !== "null"
  ) {
    targetOrigin = window.location.origin;
  }

  return targetOrigin;
}

function X500V2SendParentMessage(message) {
  if (typeof window === "undefined" || !window.parent || window.parent === window) {
    return false;
  }

  var sent = false;
  var origins = ["*", X500V2ResolveParentOrigin()];

  if (window.location && window.location.origin && window.location.origin !== "null") {
    origins.push(window.location.origin);
  }

  for (var i = 0; i < origins.length; i++) {
    try {
      window.parent.postMessage(message, origins[i]);
      sent = true;
    } catch (ignoredPostError) {}
  }

  return sent;
}

function X500V2ExitParentDirectly() {
  try {
    if (typeof window === "undefined" || !window.parent || window.parent === window) {
      return false;
    }

    if (typeof window.parent.x500v2ExitApp === "function") {
      window.parent.x500v2ExitApp();
      return true;
    }

    if (typeof window.parent.requestAppExit === "function") {
      window.parent.requestAppExit();
      return true;
    }
  } catch (ignoredDirectExitError) {}

  return false;
}

function X500V2NavigateToLandingFallback() {
  try {
    if (typeof window === "undefined" || !window.location) {
      return;
    }

    var landingUrl = new URL("../", window.location.href);
    window.location.href = landingUrl.href;
  } catch (ignoredLandingFallbackError) {
    try {
      if (window.history && window.history.length > 1) {
        window.history.back();
      }
    } catch (ignoredHistoryFallbackError) {}
  }
}

mergeInto(LibraryManager.library, {
  X500V2ExitToLanding: function () {
    try {
      if (typeof window !== "undefined" && window.parent && window.parent !== window) {
        var message = {
          type: "x500v2:exit-app",
          source: "unity-webgl",
          sentAt: Date.now ? Date.now() : 0
        };

        if (X500V2ExitParentDirectly()) {
          return;
        }

        if (X500V2SendParentMessage(message)) {
          window.setTimeout(function () {
            if (!X500V2ExitParentDirectly()) {
              X500V2SendParentMessage(message);
            }
          }, 80);
        }

        return;
      }

      if (typeof window !== "undefined" && window.history && window.history.length > 1) {
        window.history.back();
        return;
      }

      X500V2NavigateToLandingFallback();
    } catch (exitError) {
      try {
        if (window.history && window.history.length > 1) {
          window.history.back();
          return;
        }
      } catch (ignoredExitError) {}

      X500V2NavigateToLandingFallback();
    }
  },

  X500V2ReportBrowserBackResult: function (handled) {
    try {
      if (typeof window !== "undefined" && window.parent && window.parent !== window) {
        window.parent.postMessage(
          { type: "x500v2:browser-back-result", handled: handled !== 0 },
          X500V2ResolveParentOrigin()
        );
      }
    } catch (ignoredBackResultError) {}
  },

  X500V2DownloadTextFile: function (fileNamePtr, mimeTypePtr, contentPtr) {
    try {
      var fileName = UTF8ToString(fileNamePtr) || "x500v2_export.txt";
      var mimeType = UTF8ToString(mimeTypePtr) || "text/plain;charset=utf-8";
      var content = UTF8ToString(contentPtr) || "";

      if (typeof window === "undefined" || typeof document === "undefined") {
        return;
      }

      var blob = new Blob([content], { type: mimeType });
      var url = window.URL.createObjectURL(blob);
      var link = document.createElement("a");
      link.href = url;
      link.download = fileName;
      link.style.display = "none";
      document.body.appendChild(link);
      link.click();

      window.setTimeout(function () {
        try {
          document.body.removeChild(link);
        } catch (ignoredRemoveError) {}
        try {
          window.URL.revokeObjectURL(url);
        } catch (ignoredRevokeError) {}
      }, 500);
    } catch (downloadError) {
      try {
        console.error("[X500V2] Could not download profiler export", downloadError);
      } catch (ignoredConsoleError) {}
    }
  }
});
