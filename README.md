# Teste do gRPC

Aplicação simples para testar o gRPC.
O projeto consistem em uma agenda para cadastrar e consultar contatos.

Contém: 
 - Um projeto para o Server
 - Um projeto console para o Client
 - Um projeto para o protobuf. Aqui são geradas as classes que o Client e o Server consomem

## Como executar

É necessário ter o .net core 3 instalado

Abra a pasta **AgendagRPCServer** e execute o comando `dotnet run`. Vai aparecer a mensagem *Servidor está em execução*

Abra a pasta **AgendagRPCClient** e execute o comando `dotnet run`. 
Vai aparecer uma lista com os comandos que podem ser executados.

Pronto, agora é só cadastrar alguns contatos na sua agenda :smile:
