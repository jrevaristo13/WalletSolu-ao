import { useState, useEffect } from 'react';
import api from './services/api';
import { Wallet, ArrowUpCircle, ArrowDownCircle, RefreshCw, Send, History } from 'lucide-react';

function App() {
  const [saldo, setSaldo] = useState(0);
  const [historico, setHistorico] = useState([]);
  const [usuarioId] = useState("66096ccf-b509-40b5-9145-7ae86488082c"); 
  const [carregando, setCarregando] = useState(true);
  const [valorInput, setValorInput] = useState("");

  const carregarDados = async () => {
    try {
      setCarregando(true);
      // Busca saldo e extrato ao mesmo tempo
      const [resSaldo, resExtrato] = await Promise.all([
        api.get(`/wallet/saldo/${usuarioId}`),
        api.get(`/wallet/extrato/${usuarioId}?dias=7`)
      ]);
      
      setSaldo(resSaldo.data.saldo);
      setHistorico(resExtrato.data.historico || []);
    } catch (error) {
      console.error("Erro ao carregar dados", error);
    } finally {
      setCarregando(false);
    }
  };

  const executarOperacao = async (tipo) => {
    if (!valorInput || valorInput <= 0) return alert("Digite um valor!");
    try {
      setCarregando(true);
      const endpoint = tipo === 'deposito' ? '/wallet/deposito' : '/wallet/saque';
      await api.post(`${endpoint}?usuarioId=${usuarioId}&valor=${valorInput}`);
      setValorInput("");
      await carregarDados();
    } catch (error) {
      alert(error.response?.data?.erro || "Erro");
    } finally {
      setCarregando(false);
    }
  };

  useEffect(() => { carregarDados(); }, []);

  return (
    <div className="container py-5">
      <div className="row justify-content-center">
        <div className="col-md-6 col-lg-5">
          
          <div className="d-flex align-items-center justify-content-between mb-4">
            <h2 className="text-white fw-bold m-0 text-neon">WalletCore</h2>
            <button onClick={carregarDados} className="btn text-white p-0">
              <RefreshCw size={20} className={carregando ? "spin" : ""} />
            </button>
          </div>

          <div className="card-neon p-4 mb-4 text-center">
            <p className="text-secondary mb-1 small fw-bold">SALDO ATUAL</p>
            <h1 className="display-4 fw-bold text-neon mb-0">
              R$ {saldo.toLocaleString('pt-BR', { minimumFractionDigits: 2 })}
            </h1>
          </div>

          <div className="mb-4">
            <input 
              type="number" 
              className="form-control bg-dark text-white border-secondary text-center py-3 rounded-4"
              placeholder="R$ 0,00"
              value={valorInput}
              onChange={(e) => setValorInput(e.target.value)}
            />
          </div>

          <div className="row g-3 mb-4">
            <div className="col-6">
              <button onClick={() => executarOperacao('deposito')} className="btn btn-dark card-neon w-100 py-3">
                <ArrowUpCircle className="text-success mb-1" />
                <div className="small fw-bold">DEPOSITAR</div>
              </button>
            </div>
            <div className="col-6">
              <button onClick={() => executarOperacao('saque')} className="btn btn-dark card-neon w-100 py-3">
                <ArrowDownCircle className="text-danger mb-1" />
                <div className="small fw-bold">SACAR</div>
              </button>
            </div>
          </div>

          {/* EXTRATO (NOVO) */}
          <div className="mt-5">
            <div className="d-flex align-items-center gap-2 mb-3">
              <History size={18} className="text-secondary" />
              <h6 className="m-0 text-secondary fw-bold">ÚLTIMAS ATIVIDADES</h6>
            </div>
            
            <div className="extrato-container d-flex flex-column gap-2">
              {historico.length === 0 ? (
                <p className="text-center text-muted small mt-3">Nenhuma transação ainda.</p>
              ) : (
                historico.map((t, index) => (
                  <div key={index} className="card-neon p-3 d-flex justify-content-between align-items-center">
                    <div>
                      <div className="small fw-bold">{t.descricao}</div>
                      <div className="extra-small text-muted">{new Date(t.data).toLocaleDateString('pt-BR')}</div>
                    </div>
                    <div className={`fw-bold ${t.tipo === 'Receita' ? 'text-success' : 'text-danger'}`}>
                      {t.tipo === 'Receita' ? '+' : '-'} R$ {t.valor.toFixed(2)}
                    </div>
                  </div>
                ))
              )}
            </div>
          </div>

        </div>
      </div>
    </div>
  );
}

export default App;