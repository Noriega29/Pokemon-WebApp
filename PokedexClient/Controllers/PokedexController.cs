﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PokedexClient.Helpers;
using PokedexClient.Models;
using PokedexClient.Models.ViewModels;
using PokedexClient.Services;
using System.Net;
using System.Net.Sockets;

namespace PokedexClient.Controllers
{
    public class PokedexController : Controller
    {
        private readonly PokemonService _pokemonService;
        private readonly TiposService _tiposService;

        public PokedexController(PokemonService pokemonService, TiposService tiposService)
        {
            _pokemonService = pokemonService;
            _tiposService = tiposService;
        }

        // GET: pokemons/
        public async Task<IActionResult> Index()
        {
            try
            {
                var response = await _pokemonService.GetPokemonsList();
                if (response.IsSuccessStatusCode)
                {
                    var pokemons = await Deserialize.DeserializePokemonsListAsync(response);
                    return View(pokemons);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (SocketException)
            {
                return View("_LostConnection");
            }
            catch (HttpRequestException)
            {
                return View("_LostConnection");
            }
        }

        // GET: pokemons/id
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var response = await _pokemonService.GetPokemonById(id);
                if (response.IsSuccessStatusCode)
                {
                    var pokemon = await Deserialize.DeserializePokemonAsync(response);
                    return View(pokemon);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (SocketException)
            {
                return View("_LostConnection");
            }
            catch (HttpRequestException)
            {
                return View("_LostConnection");
            }
        }

        // GET: pokemon/
        public async Task<IActionResult> Create()
        {
            try
            {
                ViewData["Tipos"] = new SelectList(await _tiposService.GetTiposList(), "Id", "Nombre");
                return View();
            }
            catch (SocketException)
            {
                return View("_LostConnection");
            }
            catch (HttpRequestException)
            {
                return View("_LostConnection");
            }
        }

        // POST: pokemon/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PokemonViewModel viewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var response = await _pokemonService.CreatePokemon(viewModel);
                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        ErrorApiResponses errorMessage = new ErrorApiResponses();
                        errorMessage = await Deserialize.DeserializeErrorApiResponseAsync(response);

                        if (response.StatusCode == HttpStatusCode.Conflict)
                        {
                            if (errorMessage.Status == "409x2")
                            {
                                return View("_PartialViewAlert", AlertMessages.Error409x2(viewModel.Numero));
                            }
                            if (errorMessage.Status == "409x3")
                            {
                                return View("_PartialViewAlert", AlertMessages.Error409x3(viewModel.Name));
                            }
                        }
                    }
                }
                return View(viewModel);
            }
            catch (SocketException)
            {
                return View("_LostConnection");
            }
            catch (HttpRequestException)
            {
                return View("_LostConnection");
            }
        }

        // GET: pokemon/id
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                Pokemon pokemon = new Pokemon();
                var response = await _pokemonService.GetPokemonById(id);
                if (response.IsSuccessStatusCode)
                {
                    pokemon = await Deserialize.DeserializePokemonAsync(response);
                }
                else
                {
                    return NotFound();
                }

                List<int> tiposID = new List<int>();
                foreach (var par in pokemon.Tipos)
                {
                    tiposID.Add(par.Key);
                }

                PokemonViewModel viewModel = new()
                {
                    IdPokemon = pokemon.IdPokemon,
                    Numero = pokemon.Numero,
                    Name = pokemon.Name,
                    PrimerTipo = tiposID[0],
                    SegundoTipo = tiposID.Count > 1 ? tiposID[1] : 0,
                    Description = pokemon.Description
                };

                ViewData["Tipos"] = new SelectList(await _tiposService.GetTiposList(), "Id", "Nombre");
                return View(viewModel);
            }
            catch (SocketException)
            {
                return View("_LostConnection");
            }
            catch (HttpRequestException)
            {
                return View("_LostConnection");
            }
        }

        // POST: pokemon/id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PokemonViewModel viewModel)
        {
            try
            {
                if (id != viewModel.IdPokemon)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    var response = await _pokemonService.EditPokemon(id, viewModel);
                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        ErrorApiResponses errorMessage = new ErrorApiResponses();
                        errorMessage = await Deserialize.DeserializeErrorApiResponseAsync(response);

                        if (response.StatusCode == HttpStatusCode.Conflict)
                        {
                            if (errorMessage.Status == "409x2")
                            {
                                return View("_PartialViewAlert", AlertMessages.Error409x2(viewModel.Numero));
                            }
                            if (errorMessage.Status == "409x3")
                            {
                                return View("_PartialViewAlert", AlertMessages.Error409x3(viewModel.Name));
                            }
                        }
                    }
                }
                return View(viewModel);
            }
            catch (SocketException)
            {
                return View("_LostConnection");
            }
            catch (HttpRequestException)
            {
                return View("_LostConnection");
            }
        }
        
        // GET: pokemon/id
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var response = await _pokemonService.GetPokemonById(id);
                if (response.IsSuccessStatusCode)
                {
                    var pokemon = await Deserialize.DeserializePokemonAsync(response);
                    return View(pokemon);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (SocketException)
            {
                return View("_LostConnection");
            }
            catch (HttpRequestException)
            {
                return View("_LostConnection");
            }
        }
        
        // POST: pokemon/id
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _pokemonService.DeletePokemon(id);
                return RedirectToAction(nameof(Index));
            }
            catch (SocketException)
            {
                return View("_LostConnection");
            }
            catch (HttpRequestException)
            {
                return View("_LostConnection");
            }
        }
    }
}
