using System;
using System.Collections.Generic;
using Npgsql;

public class CarroRepository
{
    private static readonly string ConnectionString = "postgresql://neondb_owner:npg_guPXC4qU3wLD@ep-round-surf-b5duwukw-pooler.c-7.us-east-2.aws.neon.tech/neondb?sslmode=require&channel_binding=require";

    private static NpgsqlConnection CriarConexao()
    {
        if (ConnectionString == "COLE_AQUI_A_CONNECTION_STRING_DO_NEON")
            throw new InvalidOperationException(
                "Cole a connection string do Neon na constante ConnectionString, no início de CarroRepository.cs.");

        return new NpgsqlConnection(NormalizarConnectionString(ConnectionString));
    }

    private static string NormalizarConnectionString(string connectionString)
    {
        connectionString = connectionString.Trim();

        if (connectionString.StartsWith("psql ", StringComparison.OrdinalIgnoreCase))
        {
            connectionString = connectionString[5..].Trim();
            if (connectionString.Length >= 2 &&
                ((connectionString[0] == '\'' && connectionString[^1] == '\'') ||
                 (connectionString[0] == '"' && connectionString[^1] == '"')))
            {
                connectionString = connectionString[1..^1];
            }
        }

        if (!connectionString.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase) &&
            !connectionString.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase))
            return connectionString;

        if (!Uri.TryCreate(connectionString, UriKind.Absolute, out var uri) || string.IsNullOrWhiteSpace(uri.Host))
            throw new InvalidOperationException(
                "A URL do Neon está inválida. Confira o formato postgresql://usuario:senha@host/banco.");

        string userInfo = uri.UserInfo;
        int separator = userInfo.IndexOf(':');
        string database = Uri.UnescapeDataString(uri.AbsolutePath.Trim('/'));
        if (separator <= 0 || string.IsNullOrWhiteSpace(database))
            throw new InvalidOperationException(
                "A URL do Neon precisa conter usuário, senha e nome do banco.");

        var builder = new NpgsqlConnectionStringBuilder
        {
            Host = uri.Host,
            Port = uri.IsDefaultPort ? 5432 : uri.Port,
            Database = database,
            Username = Uri.UnescapeDataString(userInfo[..separator]),
            Password = Uri.UnescapeDataString(userInfo[(separator + 1)..]),
            SslMode = SslMode.Require
        };

        foreach (string parameter in uri.Query.TrimStart('?').Split('&', StringSplitOptions.RemoveEmptyEntries))
        {
            string[] pair = parameter.Split('=', 2);
            string key = Uri.UnescapeDataString(pair[0].Replace('+', ' '));
            string value = pair.Length > 1
                ? Uri.UnescapeDataString(pair[1].Replace('+', ' '))
                : "";

            switch (key.ToLowerInvariant())
            {
                case "sslmode":
                    builder["SSL Mode"] = value;
                    break;
                case "channel_binding":
                    builder["Channel Binding"] = value;
                    break;
                default:
                    throw new InvalidOperationException(
                        $"A URL contém o parâmetro '{key}', que não é suportado. Use uma connection string no formato Host=...;Database=...;Username=...;Password=...;SSL Mode=Require.");
            }
        }

        return builder.ConnectionString;
    }

    // CREATE
    public void Inserir(Carro c)
    {
        using var conn = CriarConexao();
        conn.Open();
        string sql = @"INSERT INTO carro
            (placa, numero_chassi, marca, modelo, ano, cor, combustivel, capacidade, motor, velocidade)
            VALUES (@placa, @chassi, @marca, @modelo, @ano, @cor, @comb, @cap, @motor, @vel)";
        using var cmd = new NpgsqlCommand(sql, conn);
        Preencher(cmd, c);
        cmd.ExecuteNonQuery();
    }

    // READ (listar todos)
    public List<Carro> Listar()
    {
        var lista = new List<Carro>();
        using var conn = CriarConexao();
        conn.Open();
        using var cmd = new NpgsqlCommand("SELECT * FROM carro ORDER BY id", conn);
        using var r = cmd.ExecuteReader();
        while (r.Read()) lista.Add(Mapear(r));
        return lista;
    }

    // READ (pesquisar por placa ou marca)
    public List<Carro> Pesquisar(string termo)
    {
        var lista = new List<Carro>();
        using var conn = CriarConexao();
        conn.Open();
        string sql = "SELECT * FROM carro WHERE placa ILIKE @t OR marca ILIKE @t OR modelo ILIKE @t";
        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("t", $"%{termo}%");
        using var r = cmd.ExecuteReader();
        while (r.Read()) lista.Add(Mapear(r));
        return lista;
    }

    // UPDATE
    public bool Atualizar(Carro c)
    {
        using var conn = CriarConexao();
        conn.Open();
        string sql = @"UPDATE carro SET placa=@placa, numero_chassi=@chassi, marca=@marca,
            modelo=@modelo, ano=@ano, cor=@cor, combustivel=@comb, capacidade=@cap,
            motor=@motor, velocidade=@vel WHERE id=@id";
        using var cmd = new NpgsqlCommand(sql, conn);
        Preencher(cmd, c);
        cmd.Parameters.AddWithValue("id", c.Id);
        return cmd.ExecuteNonQuery() > 0;
    }

    // DELETE
    public bool Excluir(int id)
    {
        using var conn = CriarConexao();
        conn.Open();
        using var cmd = new NpgsqlCommand("DELETE FROM carro WHERE id=@id", conn);
        cmd.Parameters.AddWithValue("id", id);
        return cmd.ExecuteNonQuery() > 0;
    }

    private static void Preencher(NpgsqlCommand cmd, Carro c)
    {
        cmd.Parameters.AddWithValue("placa", c.Placa);
        cmd.Parameters.AddWithValue("chassi", c.NumeroChassi);
        cmd.Parameters.AddWithValue("marca", c.Marca);
        cmd.Parameters.AddWithValue("modelo", c.Modelo);
        cmd.Parameters.AddWithValue("ano", c.Ano);
        cmd.Parameters.AddWithValue("cor", c.Cor);
        cmd.Parameters.AddWithValue("comb", c.Combustivel);
        cmd.Parameters.AddWithValue("cap", c.Capacidade);
        cmd.Parameters.AddWithValue("motor", c.Motor);
        cmd.Parameters.AddWithValue("vel", c.Velocidade);
    }

    private static Carro Mapear(NpgsqlDataReader r) => new Carro
    {
        Id = r.GetInt32(r.GetOrdinal("id")),
        Placa = r.GetString(r.GetOrdinal("placa")),
        NumeroChassi = r.GetString(r.GetOrdinal("numero_chassi")),
        Marca = r.GetString(r.GetOrdinal("marca")),
        Modelo = r.GetString(r.GetOrdinal("modelo")),
        Ano = r.GetInt32(r.GetOrdinal("ano")),
        Cor = r.IsDBNull(r.GetOrdinal("cor")) ? "" : r.GetString(r.GetOrdinal("cor")),
        Combustivel = r.IsDBNull(r.GetOrdinal("combustivel")) ? "" : r.GetString(r.GetOrdinal("combustivel")),
        Capacidade = r.IsDBNull(r.GetOrdinal("capacidade")) ? 0 : r.GetInt32(r.GetOrdinal("capacidade")),
        Motor = r.IsDBNull(r.GetOrdinal("motor")) ? "" : r.GetString(r.GetOrdinal("motor")),
        Velocidade = r.IsDBNull(r.GetOrdinal("velocidade")) ? 0 : r.GetInt32(r.GetOrdinal("velocidade")),
    };
}
