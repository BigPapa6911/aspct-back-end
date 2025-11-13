# ASPCTS Backend - Sistema ASPCTS

## Descrição
Sistema de gerenciamento neurodesenvolvimental baseado no VB-MAPP (Verbal Behavior Milestones Assessment and Placement Program) com funcionalidades para psicólogos e responsáveis, incluindo protocolo ABA detalhado para registro de sessões.

## Funcionalidades Principais

### Para Psicólogos:
- ✅ Gerenciamento completo de perfis de crianças
- ✅ Condução de avaliações VB-MAPP (Marcos, Barreiras, Transição)
- ✅ Documentação detalhada de sessões
- ✅ **[NOVO]** Registro de dados do protocolo ABA por sessão
- ✅ Criação e gerenciamento de planos de intervenção
- ✅ **[NOVO]** Geração de relatórios com estatísticas automáticas
- ✅ **[NOVO]** Análise de progresso por área de habilidade
- ✅ Comunicação segura com responsáveis

### Para Responsáveis:
- ✅ Visualização do progresso da criança
- ✅ Acesso a resumos de sessões compartilhadas
- ✅ **[NOVO]** Visualização de dados do protocolo ABA (sessões compartilhadas)
- ✅ Visualização de relatórios compartilhados com estatísticas
- ✅ Comunicação com o psicólogo

## Protocolo ABA - Áreas de Habilidade

O sistema agora registra dados detalhados do protocolo ABA para cada sessão:

1. **Atenção/Foco** - Capacidade de manter atenção e concentração
2. **Imitação** - Habilidades de imitação motora e verbal
3. **Contato (Tato)** - Nomeação e identificação de objetos
4. **Atividades de Mesa** - Desempenho em tarefas estruturadas
5. **Independência/Autonomia** - Habilidades de autocuidado e autonomia
6. **Tempo** - Registro de tempo de engajamento

Cada área registra:
- Número de acertos
- Total de tentativas
- Percentual calculado automaticamente
- Notas específicas do protocolo

## Configuração do Projeto

### 1. Pré-requisitos
- .NET 8.0 SDK ou superior
- SQL Server Express ou SQL Server
- Visual Studio 2022 ou VS Code

### 2. Configuração do Banco de Dados
1. Instale o SQL Server Express
2. Atualize a connection string no `appsettings.json` se necessário:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=.\\SQLEXPRESS;Database=AspctsBD;Trusted_Connection=true;TrustServerCertificate=true;"
}
```

3. **Se o banco já existe** (recomendado):
```bash
# Marcar migration inicial como aplicada (execute no SQL Server)
INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES ('20250902133611_InitialCreate', '9.0.8');

# Criar migration para novas tabelas
dotnet ef migrations add AddSessionProtocolData

# Aplicar migration
dotnet ef database update
```

4. **Se começar do zero**:
```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### 3. Executar o Projeto
```bash
# Restaurar pacotes
dotnet restore

# Executar projeto
dotnet run
```

### 4. Dados de Teste
O sistema já possui um DataSeeder que cria:
- **Psicólogo padrão**: dr.silva@example.com / senha: 123456
- **Responsáveis padrão**: joao.santos@example.com, maria.santos@example.com / senha: 123456
- **Criança de exemplo**: Pedro Santos
- **Marcos e habilidades do VB-MAPP**

## Endpoints da API

### Autenticação
- `POST /api/auth/login` - Login de usuário
- `POST /api/auth/register` - Registro de usuário
- `POST /api/auth/validate-token` - Validação de token

### Crianças
- `GET /api/children` - Lista crianças do usuário
- `GET /api/children/{id}` - Detalhes de uma criança
- `POST /api/children` - Criar criança (Psicólogo)
- `PUT /api/children/{id}` - Atualizar criança (Psicólogo)
- `DELETE /api/children/{id}` - Remover criança (Psicólogo)

### Avaliações
- `GET /api/assessments/{id}` - Detalhes de avaliação
- `GET /api/assessments/child/{childId}` - Avaliações de uma criança
- `GET /api/assessments/child/{childId}/progress` - Dados de progresso
- `POST /api/assessments/milestones` - Criar avaliação de marcos (Psicólogo)
- `POST /api/assessments/barriers` - Criar avaliação de barreiras (Psicólogo)
- `POST /api/assessments/transition` - Criar avaliação de transição (Psicólogo)

### Sessões
- `GET /api/sessions/{id}` - Detalhes de sessão (inclui dados do protocolo)
- `GET /api/sessions/child/{childId}` - Sessões de uma criança
- `POST /api/sessions` - Criar sessão (Psicólogo)
- `PUT /api/sessions/{id}` - Atualizar sessão (Psicólogo)
- `DELETE /api/sessions/{id}` - Remover sessão (Psicólogo)
- `PATCH /api/sessions/{id}/share` - Compartilhar com responsáveis (Psicólogo)

### 🆕 Protocolo de Sessão (Novo)
- `GET /api/sessions/{sessionId}/protocol` - Buscar dados do protocolo ABA
- `POST /api/sessions/{sessionId}/protocol` - Criar dados do protocolo (Psicólogo)
- `PUT /api/sessions/{sessionId}/protocol` - Atualizar dados do protocolo (Psicólogo)
- `DELETE /api/sessions/{sessionId}/protocol` - Deletar dados do protocolo (Psicólogo)

**Exemplo de Request:**
```json
{
  "totalTrials": 2,
  "attentionCorrect": 0,
  "attentionTotal": 1,
  "imitationCorrect": 3,
  "imitationTotal": 3,
  "contactCorrect": 5,
  "contactTotal": 5,
  "deskActivitiesCorrect": 1,
  "deskActivitiesTotal": 1,
  "independenceCorrect": 4,
  "independenceTotal": 4,
  "timeRegistered": 0,
  "timeTotal": 60,
  "protocolNotes": "Primeira sessão do protocolo"
}
```

### Comunicação
- `POST /api/communication/send` - Enviar mensagem
- `GET /api/communication/child/{childId}` - Mensagens de uma criança
- `GET /api/communication/conversation/{otherUserId}/child/{childId}` - Conversa específica
- `GET /api/communication/unread` - Mensagens não lidas
- `PATCH /api/communication/{messageId}/read` - Marcar como lida
- `GET /api/communication/unread-count` - Contagem de não lidas

### 📊 Relatórios (Atualizado)
- `GET /api/reports/{id}` - Detalhes de relatório **com estatísticas automáticas**
- `GET /api/reports/child/{childId}` - Relatórios de uma criança
- `POST /api/reports` - Criar relatório (Psicólogo)
- `PATCH /api/reports/{id}/share` - Compartilhar com responsáveis (Psicólogo)
- `POST /api/reports/{id}/sessions` - **[NOVO]** Adicionar sessões ao relatório (Psicólogo)
- `DELETE /api/reports/{id}/sessions/{sessionId}` - **[NOVO]** Remover sessão do relatório (Psicólogo)
- `GET /api/reports/{id}/pdf` - Download em PDF

**Estatísticas Automáticas nos Relatórios:**
- Total de sessões e tentativas (trials)
- Percentual de acerto por área de habilidade
- Tendências de progresso (melhora/declínio)
- Highlights automáticos (pontos fortes)
- Áreas que necessitam mais prática
- Progresso geral do período

**Exemplo de Request para Criar Relatório:**
```json
{
  "childId": "guid-da-crianca",
  "startPeriod": "2025-10-01T00:00:00Z",
  "endPeriod": "2025-10-31T00:00:00Z",
  "reportType": "Mensal",
  "summaryForParent": "Resumo para os pais...",
  "clinicalNotes": "Notas clínicas detalhadas...",
  "isSharedWithParent": true,
  "sessionIds": ["session-id-1", "session-id-2"]
}
```

### Planos de Intervenção
#### 📋 Consulta de Planos:
- `GET /api/interventionplans/{id}` - Detalhes de um plano específico
- `GET /api/interventionplans/child/{childId}` - Todos os planos de uma criança
- `GET /api/interventionplans/child/{childId}/active` - Plano ativo atual

#### 👨‍⚕️ Gerenciamento (Psicólogos apenas):
- `POST /api/interventionplans` - Criar novo plano (arquiva o anterior automaticamente)
- `PUT /api/interventionplans/{id}` - Atualizar plano existente
- `PATCH /api/interventionplans/{id}/archive` - Arquivar plano manualmente

#### 🎯 Gerenciamento de Metas:
- `POST /api/interventionplans/{id}/goals` - Adicionar nova meta ao plano
- `PUT /api/interventionplans/goals/{goalId}` - Atualizar meta específica

## Funcionalidades Implementadas:

### ✅ Controle de Acesso:
- Psicólogos: acesso completo aos planos das suas crianças
- Responsáveis: visualização dos planos (somente leitura)
- Validação de permissões por criança

### ✅ Lógica de Negócio:
- Arquivamento automático do plano anterior ao criar novo
- Status tracking (Active, Archived, Completed)
- Versionamento de planos com histórico completo
- Metas estruturadas com critérios de medição

### ✅ Protocolo ABA:
- Registro detalhado por área de habilidade
- Cálculo automático de percentuais
- Vinculação de sessões a relatórios
- Estatísticas consolidadas por período
- Análise de tendências de progresso

### ✅ Validações:
- Verificação se criança pertence ao psicólogo
- Validação de dados de entrada
- Tratamento de erros específicos
- Prevenção de duplicação de dados do protocolo

## Arquitetura

### Camadas:
- **Controllers**: Endpoints da API
- **Services**: Lógica de negócio
- **Repositories**: Acesso a dados
- **Models**: Entidades e DTOs
- **Middleware**: Tratamento de erros e JWT
- **Helpers**: Utilitários e AutoMapper

### Segurança:
- ✅ Autenticação JWT
- ✅ Autorização baseada em roles (Psychologist/Parent)
- ✅ Criptografia de senhas com BCrypt
- ✅ Validação de acesso a recursos
- ✅ Middleware de tratamento de erros
- ✅ Validação de propriedade de recursos

### Banco de Dados:
- Entity Framework Core com SQL Server
- Relacionamentos configurados (One-to-One, One-to-Many)
- Soft delete para crianças
- Histórico completo de avaliações e sessões
- **[NOVO]** Tabela SessionProtocolData para dados do protocolo ABA
- **[NOVO]** Relacionamento Report ↔ SessionProtocolData

### Estrutura de Entidades:

```
User
├── Psychologist
│   ├── Children (many)
│   ├── Sessions (many)
│   ├── Assessments (many)
│   ├── Reports (many)
│   └── InterventionPlans (many)
└── Parent
    └── Children (many)

Child
├── Sessions (many)
│   └── SessionProtocolData (one) [NOVO]
├── Assessments (many)
├── Reports (many)
└── InterventionPlans (many)

Report [ATUALIZADO]
└── SessionsProtocolData (many)
```

## Próximos Passos para Produção

1. **Configurar HTTPS** adequadamente
2. **Implementar logging** com Serilog
3. **Adicionar testes unitários** e de integração
4. **Configurar CI/CD**
5. **Implementar geração de PDF** com iTextSharp (incluindo gráficos de estatísticas)
6. **Adicionar validação de entrada** mais robusta
7. **Configurar rate limiting**
8. **Implementar notificações em tempo real** com SignalR
9. **Adicionar backup automático** do banco de dados
10. **Configurar monitoramento** e métricas
11. **[NOVO]** Implementar exportação de relatórios em Excel
12. **[NOVO]** Adicionar gráficos de progresso por área de habilidade
13. **[NOVO]** Criar dashboard com métricas consolidadas

## Estrutura para Frontend React

O frontend pode consumir esta API e implementar:
- Dashboard para psicólogos e responsáveis
- Formulários de avaliação VB-MAPP
- **[NOVO]** Formulário de registro do protocolo ABA
- **[NOVO]** Gráficos interativos de progresso por área de habilidade
- **[NOVO]** Visualização de estatísticas e tendências
- Sistema de mensagens em tempo real
- Geração e visualização de relatórios
- **[NOVO]** Tabelas comparativas de desempenho por sessão

### Componentes Sugeridos:

```
📊 Dashboard
├── Resumo de estatísticas
├── Gráfico de progresso por área
└── Lista de sessões recentes

📝 Registro de Sessão
├── Dados básicos da sessão
└── Formulário de protocolo ABA
    ├── Atenção (acertos/total)
    ├── Imitação (acertos/total)
    ├── Contato (acertos/total)
    ├── Atividades de Mesa (acertos/total)
    ├── Independência (acertos/total)
    └── Tempo (registrado/total)

📈 Relatórios
├── Seleção de período
├── Seleção de sessões
├── Estatísticas automáticas
│   ├── Total de sessões
│   ├── Percentuais por área
│   ├── Tendências
│   └── Highlights
└── Exportação (PDF/Excel)
```

## Fluxo de Uso Completo

### 1️⃣ Psicólogo cria uma sessão:
```http
POST /api/sessions
```

### 2️⃣ Psicólogo adiciona dados do protocolo:
```http
POST /api/sessions/{sessionId}/protocol
```

### 3️⃣ Psicólogo cria relatório mensal:
```http
POST /api/reports
{
  "sessionIds": [...],
  "startPeriod": "...",
  "endPeriod": "..."
}
```

### 4️⃣ Sistema calcula estatísticas automaticamente

### 5️⃣ Psicólogo compartilha com pais:
```http
PATCH /api/reports/{id}/share
```

### 6️⃣ Pais visualizam progresso da criança

## Troubleshooting

### Problema: Erro ao aplicar migrations
```bash
# Solução: Marcar migration inicial como aplicada
INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES ('20250902133611_InitialCreate', '9.0.8');
```

### Problema: Tabelas já existem
```bash
# Solução: Criar apenas nova migration
dotnet ef migrations add AddSessionProtocolData
dotnet ef database update
```

## Suporte

Para dúvidas ou problemas, consulte:
- Documentação do ASPCTS
- [Entity Framework Core docs](https://docs.microsoft.com/ef/core/)
- [ASP.NET Core docs](https://docs.microsoft.com/aspnet/core/)
- [JWT Authentication docs](https://jwt.io/)

## Changelog

### Versão 2.0.0 (Atual)
- ✨ Adicionado sistema de protocolo ABA para sessões
- ✨ Relatórios com estatísticas automáticas
- ✨ Análise de progresso por área de habilidade
- ✨ Vinculação de sessões a relatórios
- ✨ Cálculo automático de tendências
- ✨ Highlights e áreas de melhoria automáticos
- 🔧 Melhorias na estrutura do banco de dados
- 🔧 Novos endpoints para gerenciamento de protocolo

### Versão 1.0.0
- ✅ Sistema base com VB-MAPP
- ✅ Autenticação e autorização
- ✅ CRUD de crianças, sessões e avaliações
- ✅ Sistema de comunicação
- ✅ Planos de intervenção