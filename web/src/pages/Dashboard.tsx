import { useEffect, useState } from "react"
import { api } from "../api"
import FileCard from "../components/FileCard"
import { Doc } from "../types"

export default function Dashboard() {
  const [docs,setDocs]=useState<Doc[]>([])
  useEffect(()=>{ api.get("/search/docs").then(r=>setDocs(r.data)) },[])
  return <div style={{maxWidth:960,margin:"20px auto",display:"grid",gap:12}}>
    <h2>Documents</h2>
    <div style={{display:"grid",gap:12}}>{docs.map(d=><FileCard key={d.id} d={d} />)}</div>
  </div>
}
