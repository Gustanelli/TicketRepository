
using Npgsql;
using System;
using System.Collections.Generic;

namespace DotIA.Repositories
{
    public class TicketRepository
    {
        private readonly string _connectionString;

        public TicketRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        // MÉTODO 1: READ com filtro - Buscar um ticket por ID
        public string BuscarTicketPorId(int idTicket)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            var cmd = new NpgsqlCommand(
                "SELECT ID_TICKET, DESCRICAO_PROBLEMA, SOLUCAO FROM TICKETS WHERE ID_TICKET = @id;", 
                connection);
            cmd.Parameters.AddWithValue("@id", idTicket);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                var id = reader.GetInt32(0);
                var descricao = reader.IsDBNull(1) ? "Sem descrição" : reader.GetString(1);
                var solucao = reader.IsDBNull(2) ? "Sem solução ainda" : reader.GetString(2);
                return $"Ticket {id}:\nDescrição: {descricao}\nSolução: {solucao}";
            }

            return $"Ticket com ID {idTicket} não encontrado.";
        }

        // MÉTODO 2: DELETE - Remover um ticket do banco
        public bool DeletarTicket(int idTicket)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            var cmd = new NpgsqlCommand(
                "DELETE FROM TICKETS WHERE ID_TICKET = @id;", connection);
            cmd.Parameters.AddWithValue("@id", idTicket);

            int linhasAfetadas = cmd.ExecuteNonQuery();
            return linhasAfetadas > 0;
        }

        // MÉTODO 3: UPDATE - Atualizar a solução de um ticket
        public void AtualizarSolucaoTicket(int idTicket, string novaSolucao)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            var cmd = new NpgsqlCommand(
                "UPDATE TICKETS SET SOLUCAO = @solucao, DATA_ENCERRAMENTO = NOW() WHERE ID_TICKET = @id;", 
                connection);
            cmd.Parameters.AddWithValue("@solucao", novaSolucao);
            cmd.Parameters.AddWithValue("@id", idTicket);

            cmd.ExecuteNonQuery();
        }
    }
}
