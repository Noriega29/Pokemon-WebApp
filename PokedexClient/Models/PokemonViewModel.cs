﻿namespace PokedexClient.Models
{
    public class PokemonViewModel
    {
        public int IdPokemon { get; set; }
        public int Numero { get; set; }
        public string Name { get; set; }
        public List<int> Tipos { get; set; }
        public string Description { get; set; }
    }
}
