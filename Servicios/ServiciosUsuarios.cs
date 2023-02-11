namespace Presupuesto.Servicios
{
    public interface IServiciosUsuarios
    {
        int obtnerUsuarioid();
    }

    public class ServiciosUsuarios:IServiciosUsuarios
    {

        public int obtnerUsuarioid()
        {

            return 1;
        }

    }
}
