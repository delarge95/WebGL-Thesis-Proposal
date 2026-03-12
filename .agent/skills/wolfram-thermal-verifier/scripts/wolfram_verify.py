#!/usr/bin/env python3
import argparse
import json
import os
import sys
import urllib.error
import urllib.parse
import urllib.request


ENDPOINTS = {
    "website": "https://www.wolframalpha.com/input",
    "short": "http://api.wolframalpha.com/v1/result",
    "full": "http://api.wolframalpha.com/v2/query",
    "llm": "https://www.wolframalpha.com/api/v1/llm-api",
}


def build_params(args, appid):
    if args.endpoint == "website":
        return {"i": args.query}

    if not appid:
        raise ValueError(
            "The selected endpoint requires an AppID. Set WOLFRAM_APP_ID or pass --appid."
        )

    if args.endpoint == "short":
        params = {"appid": appid, "i": args.query}
        if args.units:
            params["units"] = args.units
        return params

    if args.endpoint == "full":
        params = {
            "appid": appid,
            "input": args.query,
            "output": "json",
            "format": "plaintext",
        }
        if args.units:
            params["units"] = args.units
        for pod_id in args.pod_id:
            params.setdefault("includepodid", [])
            params["includepodid"].append(pod_id)
        return params

    if args.endpoint == "llm":
        return {"appid": appid, "input": args.query}

    raise ValueError(f"Unsupported endpoint: {args.endpoint}")


def encode_params(params):
    if isinstance(params.get("includepodid"), list):
        items = []
        for key, value in params.items():
            if isinstance(value, list):
                for item in value:
                    items.append((key, item))
            else:
                items.append((key, value))
        return urllib.parse.urlencode(items)
    return urllib.parse.urlencode(params)


def build_url(endpoint, params):
    return f"{ENDPOINTS[endpoint]}?{encode_params(params)}"


def fetch_url(url):
    request = urllib.request.Request(url, headers={"User-Agent": "Codex-Wolfram-Workflow"})
    with urllib.request.urlopen(request, timeout=20) as response:
        payload = response.read()
        return payload.decode("utf-8", errors="replace")


def main():
    parser = argparse.ArgumentParser(
        description="Build and optionally execute WolframAlpha verification queries."
    )
    parser.add_argument("query", help="Engineering or thermal query to verify.")
    parser.add_argument(
        "--endpoint",
        choices=sorted(ENDPOINTS.keys()),
        default="website",
        help="Target endpoint. Use website for manual checks without an AppID.",
    )
    parser.add_argument("--appid", help="Explicit WolframAlpha AppID.")
    parser.add_argument(
        "--appid-env",
        default="WOLFRAM_APP_ID",
        help="Environment variable that stores the WolframAlpha AppID.",
    )
    parser.add_argument(
        "--units",
        choices=["metric", "imperial"],
        help="Optional unit system for the supported API endpoints.",
    )
    parser.add_argument(
        "--pod-id",
        action="append",
        default=[],
        help="Repeatable includepodid value for the Full Results API.",
    )
    parser.add_argument(
        "--fetch",
        action="store_true",
        help="Execute the API request. Requires an AppID for non-website endpoints.",
    )
    parser.add_argument("--save", help="Optional path to save the verification payload as JSON.")

    args = parser.parse_args()
    appid = args.appid or os.getenv(args.appid_env)

    try:
        params = build_params(args, appid)
        url = build_url(args.endpoint, params)
    except ValueError as exc:
        print(str(exc), file=sys.stderr)
        return 1

    result = {
        "query": args.query,
        "endpoint": args.endpoint,
        "website_url": build_url("website", {"i": args.query}),
        "api_url": url if args.endpoint != "website" else None,
        "fetched": False,
    }

    if args.fetch and args.endpoint != "website":
        try:
            result["response"] = fetch_url(url)
            result["fetched"] = True
        except (urllib.error.URLError, urllib.error.HTTPError, TimeoutError) as exc:
            result["error"] = str(exc)

    if args.save:
        with open(args.save, "w", encoding="utf-8") as handle:
            json.dump(result, handle, indent=2, ensure_ascii=True)

    print(json.dumps(result, indent=2, ensure_ascii=True))
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
