
## 🚨 Erros encontrados

- Após usuário mandar ou receber uma mensagem, o horário de envio e recebimento fica no local da máquina e não no horário que vem do servidor. Pelos testes realizados, isso acontece após a conversão do horário usando o DatePipe. Caso eu retire o mesmo, ele apresenta o horário correto, porém não formatado.