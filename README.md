# CRUD de Carros

Aplicativo de console em C# para cadastrar, consultar, atualizar e excluir dados de carros.

## Requisitos

- Windows com PowerShell.
- SDK do [.NET 10](https://dotnet.microsoft.com/download/dotnet/10.0).
- Acesso à internet e ao banco PostgreSQL configurado para o projeto. A tabela `carro` deve existir nesse banco.

Para conferir a instalação do .NET, execute no terminal:

```powershell
dotnet --list-sdks
dotnet --list-runtimes
```

As listas devem incluir a versão 10 do SDK e o runtime `Microsoft.NETCore.App 10.x`.

## Executar pelo VS Code

1. Abra no VS Code a pasta do projeto.
2. Abra um terminal PowerShell na pasta do projeto.
3. Inicie o aplicativo:

```powershell
.\Executar.ps1
```

Se o PowerShell bloquear o script por política de execução, permita scripts apenas na sessão atual e execute-o novamente:

```powershell
Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass -Force
.\Executar.ps1
```

Essa configuração é temporária e deixa de valer quando a sessão do PowerShell for fechada.

Como alternativa, execute o projeto diretamente:

```powershell
dotnet run --project .\CrudCarros.csproj
```

## Opções do menu

- `1` Cadastrar um carro
- `2` Listar os carros cadastrados
- `3` Pesquisar por placa, marca ou modelo
- `4` Atualizar um carro pelo ID
- `5` Excluir um carro pelo ID
- `0` Encerrar o programa

Ao cadastrar ou atualizar, informe os dados solicitados no terminal. As operações de consulta e alteração dependem da conexão com o banco PostgreSQL.

## Banco de dados

A conexão PostgreSQL é definida em `CarroRepository.cs`. Configure uma conexão válida e confirme que o banco está acessível e contém a tabela `carro` antes de usar as opções do menu que consultam ou alteram dados.
