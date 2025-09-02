# ASPCTS Backend - Sistema ASPCTS

## Descrição
Sistema de gerenciamento neurodesenvolvimental baseado no VB-MAPP (Verbal Behavior Milestones Assessment and Placement Program) com funcionalidades para psicólogos e responsáveis.

## Funcionalidades Principais

### Para Psicólogos:
- ✅ Gerenciamento completo de perfis de crianças
- ✅ Condução de avaliações VB-MAPP (Marcos, Barreiras, Transição)
- ✅ Documentação detalhada de sessões
- ✅ Criação e gerenciamento de planos de intervenção
- ✅ Geração de relatórios de progresso
- ✅ Comunicação segura com responsáveis

### Para Responsáveis:
- ✅ Visualização do progresso da criança
- ✅ Acesso a resumos de sessões compartilhadas
- ✅ Visualização de relatórios compartilhados
- ✅ Comunicação com o psicólogo

## Configuração do Projeto

### 1. Pré-requisitos
- .NET 8.0 SDK
- SQL Server Express
- Visual Studio 2022 ou VS Code

### 2. Configuração do Banco de Dados
1. Instale o SQL Server Express
2. Atualize a connection string no `appsettings.json` se necessário:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=.\\SQLEXPRESS;Database=AspctsBD;Trusted_Connection=true;TrustServerCertificate=true;"
}
```
3. Execute o Comando
```bash
dotnet ef migrations add InitialCreate
```

### 3. Executar o Projeto
```bash
# Restaurar pacotes
dotnet restore

# Executar migrações (o EnsureCreated já está configurado)
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
- `GET /api/sessions/{id}` - Detalhes de sessão
- `GET /api/sessions/child/{childId}` - Sessões de uma criança
- `POST /api/sessions` - Criar sessão (Psicólogo)
- `PUT /api/sessions/{id}` - Atualizar sessão (Psicólogo)
- `DELETE /api/sessions/{id}` - Remover sessão (Psicólogo)
- `PATCH /api/sessions/{id}/share` - Compartilhar com responsáveis (Psicólogo)

### Comunicação
- `POST /api/communication/send` - Enviar mensagem
- `GET /api/communication/child/{childId}` - Mensagens de uma criança
- `GET /api/communication/conversation/{otherUserId}/child/{childId}` - Conversa específica
- `GET /api/communication/unread` - Mensagens não lidas
- `PATCH /api/communication/{messageId}/read` - Marcar como lida
- `GET /api/communication/unread-count` - Contagem de não lidas

### Relatórios
- `GET /api/reports/{id}` - Detalhes de relatório
- `GET /api/reports/child/{childId}` - Relatórios de uma criança
- `POST /api/reports` - Criar relatório (Psicólogo)
- `PATCH /api/reports/{id}/share` - Compartilhar com responsáveis (Psicólogo)
- `GET /api/reports/{id}/pdf` - Download em PDF

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

### ✅ Validações:

- Verificação se criança pertence ao psicólogo

- Validação de dados de entrada

- Tratamento de erros específicos

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

### Banco de Dados:
- Entity Framework Core com SQL Server
- Relacionamentos configurados
- Soft delete para crianças
- Histórico completo de avaliações e sessões

## Próximos Passos para Produção

1. **Configurar HTTPS** adequadamente
2. **Implementar logging** com Serilog
3. **Adicionar testes unitários** e de integração
4. **Configurar CI/CD** 
5. **Implementar geração de PDF** com iTextSharp
6. **Adicionar validação de entrada** mais robusta
7. **Configurar rate limiting**
8. **Implementar notificações em tempo real** com SignalR
9. **Adicionar backup automático** do banco de dados
10. **Configurar monitoramento** e métricas

## Estrutura para Frontend React

O frontend pode consumir esta API e implementar:
- Dashboard para psicólogos e responsáveis
- Formulários de avaliação VB-MAPP
- Gráficos de progresso interativos
- Sistema de mensagens em tempo real
- Geração e visualização de relatórios

## Suporte

Para dúvidas ou problemas, consulte:
- Documentação do ASPCTS
- Entity Framework Core docs
- ASP.NET Core docs
- JWT Authentication docs
