using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Presupuesto.Models;
using Presupuesto.Servicios;

namespace Presupuesto.Controllers
{
    public class TiposCuentasController : Controller
    {
        private readonly IRepositorioTiposCuentas repositorioTiposCuentas;
        private readonly IServiciosUsuarios serviciosUsuarios;

        public TiposCuentasController(IRepositorioTiposCuentas repositorioTiposCuentas
            , IServiciosUsuarios serviciosUsuarios) {
            this.repositorioTiposCuentas = repositorioTiposCuentas;
            this.serviciosUsuarios = serviciosUsuarios;
        }


        public async Task<IActionResult> Index()
        {
            var usuarioid = serviciosUsuarios.obtnerUsuarioid();

            var tiposCuentas = await repositorioTiposCuentas.Obtener(usuarioid);
            return View(tiposCuentas);
        }


        public IActionResult Crear() {


            return View();
        }




        [HttpPost]
        public async Task<IActionResult> Editar(TipoCuenta tipoCuenta)
        {
            var usuarioid = serviciosUsuarios.obtnerUsuarioid();
            var tipocuentaExiste = await repositorioTiposCuentas.ObtenerPorId(tipoCuenta.Id, usuarioid);
            
            if(tipocuentaExiste is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            await repositorioTiposCuentas.Actualizar(tipoCuenta);
            return RedirectToAction("Index");

        }
        
        [HttpPost]
        public async Task<IActionResult> Crear(TipoCuenta tipoCuenta) {
            //validacion para verificar si modelo tipo cuenta es valido si no es asi se llenara el data con los datos insertados anteriormente
            if (!ModelState.IsValid)
            {
                return View(tipoCuenta);
            }
            tipoCuenta.UsuarioId = serviciosUsuarios.obtnerUsuarioid();


            //validar si ya existe una cuenta
            var yaExisteTipoCuenta =
                await repositorioTiposCuentas.Existe(tipoCuenta.Nombre, tipoCuenta.UsuarioId);

            if (yaExisteTipoCuenta)
            {
                ModelState.AddModelError(nameof(tipoCuenta.Nombre), $"El nombre {tipoCuenta.Nombre} ya existe");

                return View(tipoCuenta);

            }

            await repositorioTiposCuentas.Crear(tipoCuenta);


            return RedirectToAction("Index");
        }

        [HttpGet] 
        public async Task<IActionResult> Editar (int id)
        {
            var usuarioId = serviciosUsuarios.obtnerUsuarioid();
            var tipoCuenta = await repositorioTiposCuentas.ObtenerPorId(id, usuarioId);
            if (tipoCuenta is null)
            {
                return RedirectToAction("NoEncontrado","Home");

            }
            return View(tipoCuenta);    

        }

        public async Task<IActionResult> Borrar(int id)
        {

            var usuarioid = serviciosUsuarios.obtnerUsuarioid();
            var tipoCuenta = await repositorioTiposCuentas.ObtenerPorId(id, usuarioid);

            //es nulo o no tiene permiso de visualizarlo
            if (tipoCuenta is null)
            {

                return RedirectToAction("NoEncontrado", "Home");
            }
            return View(tipoCuenta);
        }

        [HttpPost]
        public async Task<IActionResult> BorrarTipoCuenta(int id)
        {

            var usuarioid = serviciosUsuarios.obtnerUsuarioid();
            var tipoCuenta = await repositorioTiposCuentas.ObtenerPorId(id, usuarioid);

            if (tipoCuenta is null)
            {

                return RedirectToAction("NoEncontrado", "Home");
            }
            await repositorioTiposCuentas.Borrar(id);
            return RedirectToAction("Index");
        }


        [HttpGet] // validaccion frond   enlazar propiedad a tipocuenta model
        public async Task<ActionResult> VerificarExisteTipoCuenta(string nombre)
        {

            var usuarioid = serviciosUsuarios.obtnerUsuarioid();
            var yaExisteTipoCuenta = await repositorioTiposCuentas.Existe(nombre, usuarioid);
            if (yaExisteTipoCuenta)
            {
                return Json($"El nombre {nombre} ya existe");
            }
            return Json(true);
        }
    
       
    }  
}
