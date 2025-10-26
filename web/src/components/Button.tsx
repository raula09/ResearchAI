import React from "react"
type P = React.ButtonHTMLAttributes<HTMLButtonElement>
export default function Button(p: P) { return <button {...p} style={{padding:"10px 14px",borderRadius:8,border:"1px solid #ddd",background:"#111",color:"#fff"}} /> }
