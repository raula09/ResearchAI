import { useState } from "react"
import { api } from "../api"
import ChatBubble from "../components/ChatBubble"
import Button from "../components/Button"

type Msg={role:"user"|"ai",text:string}
export default function Chat(){
  const [input,setInput]=useState("")
  const [messages,setMessages]=useState<Msg[]>([])
  async function send(){
    const u:Msg={role:"user",text:input}
    setMessages(m=>[...m,u])
    setInput("")
    const r=await api.post("/chat/ask",{message:u.text})
    const a:Msg={role:"ai",text:r.data.answer}
    setMessages(m=>[...m,a])
  }
  return <div style={{maxWidth:900,margin:"20px auto"}}>
    <h2>Chat</h2>
    <div>{messages.map((m,i)=><ChatBubble key={i} role={m.role} text={m.text} />)}</div>
    <div style={{display:"grid",gridTemplateColumns:"1fr auto",gap:8,marginTop:12}}>
      <input value={input} onChange={e=>setInput(e.target.value)} placeholder="Ask your knowledge base" />
      <Button onClick={send}>Send</Button>
    </div>
  </div>
}
