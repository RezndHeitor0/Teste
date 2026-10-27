using System;
using System.Collections.Generic;

var repo = new CarroRepository();

while (true)
{
    Console.WriteLine("\n=== CRUD DE CARROS ===");
    Console.WriteLine("1 - Cadastrar\n2 - Listar\n3 - Pesquisar\n4 - Atualizar\n5 - Excluir\n0 - Sair");
    Console.Write("Opção: ");
    string? op = Console.ReadLine();

    try
    {
        switch (op)
        {
            case "1": repo.Inserir(LerCarro()); Console.WriteLine("Cadastrado!"); break;
            case "2": Mostrar(repo.Listar()); break;
            case "3":
                Console.Write("Termo: ");
                Mostrar(repo.Pesquisar(Console.ReadLine() ?? ""));
                break;
            case "4":
                Console.Write("ID do carro: ");
                if (!int.TryParse(Console.ReadLine(), out int id))
                {
                    Console.WriteLine("ID inválido.");
                    break;
                }
                var c = LerCarro();
                c.Id = id;
                Console.WriteLine(repo.Atualizar(c) ? "Atualizado!" : "ID não encontrado.");
                break;
            case "5":
                Console.Write("ID do carro: ");
                Console.WriteLine(repo.Excluir(int.Parse(Console.ReadLine() ?? "0")) ? "Excluído!" : "ID não encontrado.");
                break;
            case "0": return;
            default: Console.WriteLine("Opção inválida."); break;
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Erro: {ex.Message}");
    }
}

static Carro LerCarro()
{
    var c = new Carro();
    Console.Write("Placa: "); c.Placa = Console.ReadLine() ?? "";
    Console.Write("Chassi: "); c.NumeroChassi = Console.ReadLine() ?? "";
    Console.Write("Marca: "); c.Marca = Console.ReadLine() ?? "";
    Console.Write("Modelo: "); c.Modelo = Console.ReadLine() ?? "";
    Console.Write("Ano: "); c.Ano = int.Parse(Console.ReadLine() ?? "0");
    Console.Write("Cor: "); c.Cor = Console.ReadLine() ?? "";
    Console.Write("Combustível: "); c.Combustivel = Console.ReadLine() ?? "";
    Console.Write("Capacidade: "); c.Capacidade = int.Parse(Console.ReadLine() ?? "0");
    Console.Write("Motor: "); c.Motor = Console.ReadLine() ?? "";
    Console.Write("Velocidade: "); c.Velocidade = int.Parse(Console.ReadLine() ?? "0");
    return c;
}

static void Mostrar(List<Carro> lista)
{
    if (lista.Count == 0) { Console.WriteLine("Nenhum registro."); return; }
    foreach (var c in lista)
        Console.WriteLine($"[{c.Id}] {c.Placa} | {c.Marca} {c.Modelo} {c.Ano} | {c.Cor} | {c.Combustivel} | Chassi: {c.NumeroChassi}");
}
