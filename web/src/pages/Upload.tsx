import { useState } from "react"
import Button from "../components/Button"
import { api } from "../api"

export default function Upload() {
  const [file,setFile]=useState<File|null>(null)
  const [title,setTitle]=useState("")
  const [text,setText]=useState("")
  async function uploadPdf() {
    if(!file||!title) return
    const f = new FormData()
    f.append("file",file)
    f.append("title",title)
    await api.post("/document/upload",f)
    alert("Uploaded")
  }
  async function uploadText() {
    if(!text||!title) return
    await api.post("/document/text",{title,text})
    alert("Uploaded")
  }
  return <div style={{maxWidth:720,margin:"20px auto",display:"grid",gap:16}}>
    <h2>Upload</h2>
    <input placeholder="Title" value={title} onChange={e=>setTitle(e.target.value)} />
    <div style={{display:"grid",gap:10}}>
      <input type="file" accept="application/pdf" onChange={e=>setFile(e.target.files?.[0]||null)} />
      <Button onClick={uploadPdf}>Upload PDF</Button>
    </div>
    <textarea placeholder="Paste text" value={text} onChange={e=>setText(e.target.value)} rows={10} />
    <Button onClick={uploadText}>Upload Text</Button>
  </div>
}
