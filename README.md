# StockPoint — WPF Frontend

Frontend en WPF (.NET 10) para el sistema de punto de venta del Proyecto 2 de LPAC 2026.  
Consume la API RESTful del backend (ASP.NET Core MVC) desarrollado por Joel.

## Equipo 3

| Integrante | Responsabilidad |
|---|---|
| Jared Morales | Backend: Domain + Data + SQL Server |
| Joel | Backend: Business + Controllers REST + pruebas .http |
| **Emanuel Araya** | **WPF Vista 1: Gestión del Producto (CRUD)** |
| **Adrian Barboza** | **WPF Vista 2: Registrar Orden de Trabajo** |

---

## Requisitos

- Visual Studio 2022 (o Insiders)
- .NET 10 SDK
- API del backend corriendo localmente

---

## Cómo abrir el proyecto

```
git clone https://github.com/AdrianBarbozaV/StockPoint-WPF.git
cd StockPoint-WPF
```

Abrir `StockPoint.WPF.slnx` en Visual Studio. Los paquetes NuGet se restauran automáticamente.

---

## Estructura del proyecto

```
StockPoint.WPF/
├── Models/
│   ├── Producto.cs          # Id, Codigo, Nombre, PrecioUnitario, TieneImpuesto, Stock
│   ├── Cliente.cs           # ClienteId, Nombre, Cedula
│   ├── OrdenDetalle.cs      # detalle de línea de orden (calcula Subtotal)
│   └── Orden.cs             # encabezado de orden con lista de detalles
├── ViewModels/
│   └── OrdenViewModel.cs    # ViewModel de la Vista 2 (Adrian)
├── Views/
│   └── OrdenUserControl.xaml  # Vista 2: Registrar Orden (Adrian)
├── Services/
│   └── OrdenService.cs      # HttpClient → API (clientes, productos, ordenes)
├── AppConfig.cs             # Lee appsettings.json
├── appsettings.json         # URL base de la API
└── MainWindow.xaml          # Ventana principal (carga el UserControl)
```

---

## Configurar la URL de la API

Antes de correr, editá `appsettings.json` con el puerto que use Joel en su API:

```json
{
  "ApiSettings": {
    "BaseUrl": "https://localhost:7154/"
  }
}
```

---

## Patrón MVVM

El proyecto sigue el mismo patrón del ejemplo del profesor (VideoRentWpf):

- **Models** implementan `INotifyPropertyChanged` con backing fields
- **ViewModels** extienden `ObservableObject` de `CommunityToolkit.Mvvm`
- **Commands** usan `[RelayCommand]` con `CanExecute` cuando aplica
- **Views** son `UserControl`, no `Window` independientes

---

## Paquetes NuGet instalados

| Paquete | Uso |
|---|---|
| `CommunityToolkit.Mvvm` | ObservableObject, RelayCommand, ObservableProperty |
| `Microsoft.Extensions.Configuration.Json` | Leer appsettings.json |
| `System.Net.Http.Json` | GetFromJsonAsync / PostAsJsonAsync |

---

## Branches

| Branch | Contenido |
|---|---|
| `main` | Base limpia del proyecto VS — partí de aquí |
| `feature/orden-view` | Vista 2 de Adrian (Registrar Orden) — en desarrollo |

**Para tu vista (Emanuel):** creá tu branch desde `main`:

```bash
git checkout main
git pull origin main
git checkout -b feature/gestion-producto
```

Al terminar, hacés PR hacia `main`.

---

## Lo que te toca a vos, Emanuel — Vista 1: Gestión del Producto

Tenés que crear:

- `ViewModels/ProductoViewModel.cs` — extiende `ObservableObject`, igual que `OrdenViewModel`
- `Views/ProductoUserControl.xaml` — CRUD con DataGrid + formulario (patrón maestro-detalle)
- `Views/ProductoUserControl.xaml.cs`
- `Services/ProductoService.cs` — llama a `api/productos` (GET, POST, PUT, DELETE)

Los modelos que necesitás (`Producto.cs`) ya están en `Models/`.  
Seguí el mismo patrón de `OrdenViewModel` y `OrdenService` para que sea consistente.

Para mostrar tu vista en la app, en `MainWindow.xaml` vas a agregar navegación o simplemente swapear el `UserControl` cargado — coordinamos eso cuando tengas tu branch lista.
