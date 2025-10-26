import axios from "axios"
const API_URL = import.meta.env.VITE_API_URL || "http://localhost:5200"
export const api = axios.create({ baseURL: API_URL + "/api" })
export function setToken(t: string) { api.defaults.headers.common["Authorization"] = "Bearer " + t }
