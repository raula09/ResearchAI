import { BrowserRouter, Routes, Route, Link, Navigate } from "react-router-dom"
import Login from "./pages/Login"
import Register from "./pages/Register"
import Upload from "./pages/Upload"
import Dashboard from "./pages/Dashboard"
import Chat from "./pages/Chat"
import { loadToken } from "./auth"
import { setToken } from "./api"

function Shell({children}:{children:React.ReactNode}){
  return <div>
    <div style={{display:"flex",gap:12,alignItems:"center",padding:12,borderBottom:"1px solid #eee"}}>
      <Link to="/dashboard">Dashboard</Link>
      <Link to="/upload">Upload</Link>
      <Link to="/chat">Chat</Link>
    </div>
    <div>{children}</div>
  </div>
}

function Protected({children}:{children:JSX.Element}){
  const t = loadToken()
  if(!t) return <Navigate to="/login" />
  setToken(t)
  return children
}

export default function App(){
  return <BrowserRouter>
    <Routes>
      <Route path="/login" element={<Login/>}/>
      <Route path="/register" element={<Register/>}/>
      <Route path="/" element={<Navigate to="/dashboard"/>}/>
      <Route path="/" element={<Shell><div/></Shell>}/>
      <Route path="/dashboard" element={<Protected><Shell><Dashboard/></Shell></Protected>}/>
      <Route path="/upload" element={<Protected><Shell><Upload/></Shell></Protected>}/>
      <Route path="/chat" element={<Protected><Shell><Chat/></Shell></Protected>}/>
    </Routes>
  </BrowserRouter>
}
