import { Doc } from "../types"
export default function FileCard({ d }: { d: Doc }) {
  return <div style={{border:"1px solid #eee",borderRadius:12,padding:12}}>
    <div style={{fontWeight:700}}>{d.title}</div>
    <div style={{whiteSpace:"pre-wrap",marginTop:8}}>{d.summary}</div>
    <div style={{opacity:.6,marginTop:8}}>{new Date(d.createdAt).toLocaleString()}</div>
  </div>
}
