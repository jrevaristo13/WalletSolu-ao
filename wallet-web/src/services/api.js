import axios from 'axios';

const api = axios.create({
    baseURL: 'http://localhost:5000/api' // 🔹 A porta da sua API C#
});

export default api;