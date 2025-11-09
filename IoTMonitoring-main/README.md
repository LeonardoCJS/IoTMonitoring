IoT Monitoring System - .NET
Sobre o Projeto
Objetivo do Projeto
Desenvolver um sistema de monitoramento para dispositivos IoT que permita coletar, armazenar e visualizar dados de sensores em tempo real. O sistema visa resolver problemas de monitoramento remoto de dispositivos, fornecendo uma plataforma centralizada para gerenciamento de dispositivos IoT e análise de dados coletados.

Problema a Ser Resolvido
Falta de centralização: Dispersão de dados de diferentes dispositivos IoT

Monitoramento ineficiente: Dificuldade em acompanhar o status e saúde dos dispositivos

Análise limitada: Capacidade reduzida de analisar dados históricos de sensores

Integração complexa: Dificuldade em integrar diferentes tipos de dispositivos IoT

Escopo do Projeto
Funcionalidades Principais
Gestão de Dispositivos: Cadastro, edição e remoção de dispositivos IoT

Coleta de Dados: Recebimento e armazenamento de dados de sensores

Monitoramento em Tempo Real: Acompanhamento do status dos dispositivos

Consulta Histórica: Análise de dados de sensores por período

API RESTful: Interface para integração com dispositivos e aplicações externas

Documentação Swagger: Documentação interativa da API

Limitações
Interface gráfica web (foco em API)

Notificações em tempo real (WebSockets)

Autenticação e autorização (para simplificação)

Dashboard de visualização

Requisitos
Requisitos Funcionais
ID	Requisito	Descrição	Prioridade
RF01	Cadastrar Dispositivo	Permitir cadastro de novos dispositivos IoT	Alta
RF02	Listar Dispositivos	Visualizar todos os dispositivos cadastrados	Alta
RF03	Buscar Dispositivo	Localizar dispositivo por ID ou identificador	Alta
RF04	Coletar Dados Sensor	Receber e armazenar dados de sensores	Alta
RF05	Consultar Dados Históricos	Buscar dados por dispositivo e período	Média
RF06	Atualizar Status	Alterar status do dispositivo (Online/Offline/Error)	Média
RF07	Excluir Dispositivo	Remover dispositivo do sistema	Baixa
Requisitos Não Funcionais
ID	Requisito	Descrição
RNF01	Performance	Tempo de resposta < 200ms para operações CRUD
RNF02	Escalabilidade	Suporte a múltiplos dispositivos simultâneos
RNF03	Confiabilidade	Disponibilidade de 99.5%
RNF04	Segurança	Validação de dados de entrada
RNF05	Manutenibilidade	Código limpo e documentado
RNF06	Testabilidade	Cobertura de testes > 80%
 Arquitetura da Aplicação
Clean Architecture
O projeto segue os princípios da Clean Architecture, garantindo:

Separação de responsabilidades

Baixo acoplamento entre camadas

Alta coesão dentro das camadas

Testabilidade

Independência de frameworks
