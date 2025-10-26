import React from "react"
type P = React.InputHTMLAttributes<HTMLInputElement> & { label?: string }
export default function Input(p: P) { return <div style={{display:"grid",gap:6}}><label>{p.label}</label><input {...p} /></div> }
