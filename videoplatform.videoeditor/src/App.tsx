import { getComponents, registerElements } from 'omniclip'

import './App.css'

function App() {
  registerElements(getComponents());
  return (
    <>
        <p>test</p>
    </>
  )
}

export default App
