export default function ChatBubble({ role, text }: { role: "user" | "ai", text: string }) {
  return <div style={{display:"flex",justifyContent:role==="user"?"flex-end":"flex-start",margin:"8px 0"}}>
    <div style={{maxWidth:700,background:role==="user"?"#2563eb":"#f1f5f9",color:role==="user"?"#fff":"#111",borderRadius:12,padding:12,whiteSpace:"pre-wrap"}}>{text}</div>
  </div>
}
