using CarslineApp.Models;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace CarslineApp.Services
{
    public partial class ApiService
    {
        public async Task<AvaluoResponse> CrearAvaluoAsync(CrearAvaluoRequest request)
        {
            try
            {
                Debug.WriteLine($"📋 Creando avalúo para {request.NombreCompleto} | VIN: {request.VIN}");

                var response = await _httpClient.PostAsJsonAsync(
                    $"{BaseUrl}/Avaluos/crear", request);

                var result = await response.Content.ReadFromJsonAsync<AvaluoResponse>(
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (response.IsSuccessStatusCode)
                {
                    Debug.WriteLine($"✅ Avalúo creado con ID: {result?.AvaluoId}");
                    return result ?? new AvaluoResponse { Success = false, Message = "Error al procesar respuesta" };
                }

                Debug.WriteLine($"❌ Error HTTP {response.StatusCode}: {result?.Message}");
                return result ?? new AvaluoResponse
                {
                    Success = false,
                    Message = $"Error en la solicitud: {response.StatusCode}"
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Excepción en CrearAvaluoAsync: {ex.Message}");
                return new AvaluoResponse { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        /// <summary>
        /// Registrar el equipamiento del vehículo avaluado
        /// POST api/Avaluos/equipamiento
        /// </summary>
        public async Task<EquipamientoAvaluoResponse> CrearEquipamientoAvaluoAsync(CrearEquipamientoRequest request)
        {
            try
            {
                Debug.WriteLine($"🔧 Registrando equipamiento para Avalúo ID: {request.AvaluoId}");

                var response = await _httpClient.PostAsJsonAsync(
                    $"{BaseUrl}/Avaluos/equipamiento", request);

                var result = await response.Content.ReadFromJsonAsync<EquipamientoAvaluoResponse>(
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (response.IsSuccessStatusCode)
                {
                    Debug.WriteLine("✅ Equipamiento registrado exitosamente");
                    return result ?? new EquipamientoAvaluoResponse { Success = false, Message = "Error al procesar respuesta" };
                }

                Debug.WriteLine($"❌ Error HTTP {response.StatusCode}: {result?.Message}");
                return result ?? new EquipamientoAvaluoResponse
                {
                    Success = false,
                    Message = $"Error: {response.StatusCode}"
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Excepción en CrearEquipamientoAvaluoAsync: {ex.Message}");
                return new EquipamientoAvaluoResponse { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        /// <summary>
        /// Agregar lista de reparaciones estimadas al avalúo
        /// POST api/Avaluos/reparaciones
        /// </summary>
        public async Task<ReparacionesAvaluoResponse> AgregarReparacionesAvaluoAsync(CrearReparacionesRequest request)
        {
            try
            {
                Debug.WriteLine($"🔩 Agregando {request.Reparaciones.Count} reparaciones al Avalúo ID: {request.AvaluoId}");

                var response = await _httpClient.PostAsJsonAsync(
                    $"{BaseUrl}/Avaluos/reparaciones", request);

                var result = await response.Content.ReadFromJsonAsync<ReparacionesAvaluoResponse>(
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (response.IsSuccessStatusCode)
                {
                    Debug.WriteLine($"✅ {result?.CantidadReparaciones} reparaciones agregadas. Total: {result?.TotalFormateado}");
                    return result ?? new ReparacionesAvaluoResponse { Success = false, Message = "Error al procesar respuesta" };
                }

                Debug.WriteLine($"❌ Error HTTP {response.StatusCode}: {result?.Message}");
                return result ?? new ReparacionesAvaluoResponse
                {
                    Success = false,
                    Message = $"Error: {response.StatusCode}"
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Excepción en AgregarReparacionesAvaluoAsync: {ex.Message}");
                return new ReparacionesAvaluoResponse { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        /// <summary>
        /// Agregar una sola reparación (método simplificado)
        /// </summary>
        public async Task<ReparacionesAvaluoResponse> AgregarReparacionSimpleAsync(
            int avaluoId,
            string descripcion,
            decimal costo)
        {
            var request = new CrearReparacionesRequest
            {
                AvaluoId = avaluoId,
                Reparaciones = new List<ReparacionItemRequest>
                {
                    new() { Descripcion = descripcion, CostoAproximado = costo }
                }
            };
            return await AgregarReparacionesAvaluoAsync(request);
        }

        /// <summary>
        /// Eliminar una reparación del avalúo
        /// DELETE api/Avaluos/reparacion/{id}
        /// </summary>
        public async Task<AvaluoResponse> EliminarReparacionAvaluoAsync(int reparacionId)
        {
            try
            {
                Debug.WriteLine($"🗑️ Eliminando reparación ID: {reparacionId}");

                var response = await _httpClient.DeleteAsync(
                    $"{BaseUrl}/Avaluos/reparacion/{reparacionId}");

                var result = await response.Content.ReadFromJsonAsync<AvaluoResponse>(
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (result != null)
                {
                    Debug.WriteLine(result.Success
                        ? "✅ Reparación eliminada"
                        : $"⚠️ No se pudo eliminar: {result.Message}");
                    return result;
                }

                return new AvaluoResponse
                {
                    Success = false,
                    Message = $"Error HTTP: {response.StatusCode}"
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error en EliminarReparacionAvaluoAsync: {ex.Message}");
                return new AvaluoResponse { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        /// <summary>
        /// Subir fotos del vehículo avaluado (multipart/form-data)
        /// POST api/Avaluos/fotos
        /// </summary>
        public async Task<FotosAvaluoResponse> SubirFotosAvaluoAsync(
            int avaluoId,
            List<FotoAvaluoUpload> fotos)
        {
            try
            {
                if (fotos == null || !fotos.Any())
                    return new FotosAvaluoResponse { Success = false, Message = "No hay fotos para subir" };

                Debug.WriteLine($"📷 Subiendo {fotos.Count} foto(s) para Avalúo ID: {avaluoId}");

                using var content = new MultipartFormDataContent();

                content.Add(new StringContent(avaluoId.ToString()), "AvaluoId");

                foreach (var foto in fotos)
                {
                    if (foto.ImagenBytes == null || foto.ImagenBytes.Length == 0) continue;

                    var byteContent = new ByteArrayContent(foto.ImagenBytes);
                    byteContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
                    content.Add(byteContent, "Imagenes", foto.NombreArchivo);
                    content.Add(new StringContent(foto.TipoFoto), "TiposFoto");
                }

                var response = await _httpClient.PostAsync(
                    $"{BaseUrl}/Avaluos/fotos", content);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<FotosAvaluoResponse>(
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    Debug.WriteLine($"✅ {result?.CantidadFotos} foto(s) subidas exitosamente");

                    return result ?? new FotosAvaluoResponse { Success = true, Message = "Fotos subidas correctamente" };
                }

                var error = await response.Content.ReadAsStringAsync();
                Debug.WriteLine($"❌ Error al subir fotos {response.StatusCode}: {error}");

                return new FotosAvaluoResponse
                {
                    Success = false,
                    Message = $"Error al subir fotos: {response.StatusCode}"
                };
            }
            catch (TaskCanceledException)
            {
                Debug.WriteLine("⏱️ Timeout al subir fotos");
                return new FotosAvaluoResponse { Success = false, Message = "La solicitud tardó demasiado. Verifica tu conexión." };
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Excepción en SubirFotosAvaluoAsync: {ex.Message}");
                return new FotosAvaluoResponse { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        /// <summary>
        /// Subir una sola foto (método simplificado)
        /// </summary>
        public async Task<FotosAvaluoResponse> SubirFotoSimpleAsync(
            int avaluoId,
            string tipoFoto,
            byte[] imagenBytes,
            string nombreArchivo)
        {
            return await SubirFotosAvaluoAsync(avaluoId, new List<FotoAvaluoUpload>
            {
                new() { TipoFoto = tipoFoto, ImagenBytes = imagenBytes, NombreArchivo = nombreArchivo }
            });
        }

        // ============================================
        // CONSULTAS
        // ============================================

        /// <summary>
        /// Obtener avalúo completo por ID (datos + equipamiento + reparaciones + fotos)
        /// GET api/Avaluos/{id}
        /// </summary>
        public async Task<AvaluoCompletoResponse> ObtenerAvaluoCompletoAsync(int avaluoId)
        {
            try
            {
                Debug.WriteLine($"📋 Obteniendo avalúo completo ID: {avaluoId}");

                var response = await _httpClient.GetAsync($"{BaseUrl}/Avaluos/{avaluoId}");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<AvaluoCompletoResponse>(
                        json,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    Debug.WriteLine("✅ Avalúo completo obtenido");
                    return result ?? new AvaluoCompletoResponse { Success = false, Message = "Error al procesar respuesta" };
                }

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return new AvaluoCompletoResponse { Success = false, Message = "Avalúo no encontrado" };

                return new AvaluoCompletoResponse
                {
                    Success = false,
                    Message = $"Error: {response.StatusCode}"
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error en ObtenerAvaluoCompletoAsync: {ex.Message}");
                return new AvaluoCompletoResponse { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        /// <summary>
        /// Obtener todos los avalúos de un asesor
        /// GET api/Avaluos/asesor/{asesorId}
        /// </summary>
        public async Task<AvaluosListaResponse> ObtenerAvaluosPorAsesorAsync(int asesorId)
        {
            try
            {
                Debug.WriteLine($"📋 Obteniendo avalúos del asesor ID: {asesorId}");

                var response = await _httpClient.GetAsync($"{BaseUrl}/Avaluos/asesor/{asesorId}");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<AvaluosListaResponse>(
                        json,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    Debug.WriteLine($"✅ Se obtuvieron {result?.Total ?? 0} avalúo(s)");
                    return result ?? new AvaluosListaResponse { Success = false };
                }

                return new AvaluosListaResponse
                {
                    Success = false,
                    Message = $"Error: {response.StatusCode}"
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error en ObtenerAvaluosPorAsesorAsync: {ex.Message}");
                return new AvaluosListaResponse { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        /// <summary>
        /// Autorizar precio del avalúo
        /// PUT api/Avaluos/autorizar/{id}
        /// </summary>
        public async Task<AvaluoResponse> AutorizarAvaluoAsync(int avaluoId, decimal precioAutorizado, bool vehiculoApto = true)
        {
            try
            {
                Debug.WriteLine($"✅ Autorizando avalúo {avaluoId} con precio ${precioAutorizado:N2}");

                var request = new AutorizarAvaluoRequest
                {
                    PrecioAutorizado = precioAutorizado,
                    VehiculoApto = vehiculoApto
                };

                var response = await _httpClient.PutAsJsonAsync(
                    $"{BaseUrl}/Avaluos/autorizar/{avaluoId}", request);

                var result = await response.Content.ReadFromJsonAsync<AvaluoResponse>(
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (response.IsSuccessStatusCode)
                {
                    Debug.WriteLine("✅ Avalúo autorizado exitosamente");
                    return result ?? new AvaluoResponse { Success = false };
                }

                return result ?? new AvaluoResponse
                {
                    Success = false,
                    Message = $"Error: {response.StatusCode}"
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error en AutorizarAvaluoAsync: {ex.Message}");
                return new AvaluoResponse { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        /// <summary>
        /// Obtener URL de una foto de avalúo para visualización
        /// </summary>
        public string ObtenerUrlFotoAvaluo(int fotoId) =>
            $"{BaseUrl}/Avaluos/foto/{fotoId}";
    }
}
