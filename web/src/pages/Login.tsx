import { useState } from "react"
import Input from "../components/Input"
import Button from "../components/Button"
import { api } from "../api"
import { saveToken } from "../auth"
import { useNavigate } from "react-router-dom"

export default function Login() {
  const [email,setEmail]=useState(""); const [password,setPassword]=useState(""); const nav=useNavigate()
  async function submit() {
    const r = await api.post("/auth/login",{email,password})
    saveToken(r.data.token)
    nav("/dashboard")
  }
  return <div style={{maxWidth:360,margin:"60px auto",display:"grid",gap:12}}>
    <h2>Sign in</h2>
    <Input value={email} onChange={e=>setEmail(e.target.value)} label="Email" />
    <Input value={password} onChange={e=>setPassword(e.target.value)} type="password" label="Password" />
    <Button onClick={submit}>Login</Button>
  </div>
}
