import React from 'react'
import Hero from './components/sections/Hero'
import CapacidadesSection from './components/sections/CapacidadesSection'
import TecnologiaSection from './components/sections/TecnologiaSection'
import Footer from './components/sections/Footer'
import './App.css'

function App() {
  return (
    <div className="app">
      <Hero />
      <CapacidadesSection />
      <TecnologiaSection />
      <Footer />
    </div>
  )
}

export default App
