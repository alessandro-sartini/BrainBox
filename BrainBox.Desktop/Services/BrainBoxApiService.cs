using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using BrainBox.Desktop.Models;

namespace BrainBox.Desktop.Services
{
    public class BrainBoxApiService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://localhost:7082";

        public BrainBoxApiService()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(BaseUrl)
            };
        }

        // ════════════════════════════════════════
        // METODI PER LE IDEE
        // ════════════════════════════════════════

        /// <summary>
        /// GET /api/ideas - Ottiene tutte le idee
        /// </summary>
        public async Task<List<IdeaDto>> GetIdeasAsync()
        {
            try
            {
                var ideas = await _httpClient.GetFromJsonAsync<List<IdeaDto>>("/api/ideas");
                return ideas ?? [];
            }
            catch (Exception ex)
            {
                throw new Exception($"Errore durante il caricamento delle idee: {ex.Message}");
            }
        }

        /// <summary>
        /// GET /api/ideas/{id} - Ottiene una singola idea
        /// </summary>
        public async Task<IdeaDto> GetIdeaByIdAsync(int id)
        {
            try
            {
                var idea = await _httpClient.GetFromJsonAsync<IdeaDto>($"/api/ideas/{id}");
                return idea!;
            }
            catch (Exception ex)
            {
                throw new Exception($"Errore durante il caricamento dell'idea: {ex.Message}");
            }
        }

        /// <summary>
        /// POST /api/ideas - Crea una nuova idea
        /// </summary>
        public async Task<IdeaDto> CreateIdeaAsync(CreateIdeaDto createDto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("/api/ideas", createDto);
                response.EnsureSuccessStatusCode();

                var idea = await response.Content.ReadFromJsonAsync<IdeaDto>();
                return idea!;
            }
            catch (Exception ex)
            {
                throw new Exception($"Errore durante la creazione dell'idea: {ex.Message}");
            }
        }

        /// <summary>
        /// PUT /api/ideas/{id} - Modifica un'idea esistente
        /// </summary>
        public async Task UpdateIdeaAsync(int id, UpdateIdeaDto updateDto)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"/api/ideas/{id}", updateDto);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                throw new Exception($"Errore durante la modifica dell'idea: {ex.Message}");
            }
        }

        /// <summary>
        /// DELETE /api/ideas/{id} - Elimina un'idea
        /// </summary>
        public async Task DeleteIdeaAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"/api/ideas/{id}");
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                throw new Exception($"Errore durante l'eliminazione dell'idea: {ex.Message}");
            }
        }

        // 
        // METODI PER I TEMI
        // 

        /// <summary>
        /// GET /api/themes - Ottiene tutti i temi
        /// </summary>
        public async Task<List<ThemeDto>> GetThemesAsync()
        {
            try
            {
                var themes = await _httpClient.GetFromJsonAsync<List<ThemeDto>>("/api/themes");
                return themes ?? new List<ThemeDto>();
            }
            catch (Exception ex)
            {
                throw new Exception($"Errore durante il caricamento dei temi: {ex.Message}");
            }
        }

        /// <summary>
        /// POST /api/themes - Crea un nuovo tema
        /// </summary>
        public async Task<ThemeDto> CreateThemeAsync(string themeName)
        {
            try
            {
                var createDto = new { Name = themeName };
                var response = await _httpClient.PostAsJsonAsync("/api/themes", createDto);
                response.EnsureSuccessStatusCode();

                var theme = await response.Content.ReadFromJsonAsync<ThemeDto>();
                return theme!;
            }
            catch (Exception ex)
            {
                throw new Exception($"Errore durante la creazione del tema: {ex.Message}");
            }
        }

        /// <summary>
        /// DELETE /api/themes/{id} - Elimina un tema
        /// </summary>
        public async Task DeleteThemeAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"/api/themes/{id}");
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                throw new Exception($"Errore durante l'eliminazione del tema: {ex.Message}");
            }
        }




        /// <summary>
        /// PUT /api/themes/{id} - Modifica un tema
        /// </summary>
        public async Task UpdateThemeAsync(int id, string newName)
        {
            try
            {
                var updateDto = new { Name = newName };
                var response = await _httpClient.PutAsJsonAsync($"/api/themes/{id}", updateDto);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                throw new Exception($"Errore durante la modifica del tema: {ex.Message}");
            }
        }




    }
}
