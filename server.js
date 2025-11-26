const express = require('express');
const bodyParser = require('body-parser');
const path = require('path');

const app = express();
app.use(bodyParser.json());
app.use(express.static('public')); // Serve os arquivos HTML da pasta public

// --- BANCO DE DADOS (Simulado na memória) ---
// Se reiniciar o servidor, os dados somem.
let estoque = [
    { id: 1, nome: "Teclado", quantidade: 10 },
    { id: 2, nome: "Mouse", quantidade: 25 }
];

// --- ROTAS DA API (IPI) ---

// 1. Rota para LISTAR itens
app.get('/api/itens', (req, res) => {
    res.json(estoque);
});

// 2. Rota para ADICIONAR item
app.post('/api/itens', (req, res) => {
    const novoItem = req.body;
    // Cria um ID simples e adiciona ao array
    novoItem.id = estoque.length + 1;
    estoque.push(novoItem);
    
    console.log("Item adicionado:", novoItem);
    res.json({ message: "Sucesso!", item: novoItem });
});

// Iniciar Servidor
app.listen(3000, () => {
    console.log('Servidor rodando em http://localhost:3000');
});