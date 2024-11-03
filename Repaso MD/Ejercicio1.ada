-- Resolver el siguiente problema. La página web del Banco Central exhibe las diferentes cotizaciones
-- del dólar oficial de 20 bancos del país, tanto para la compra como para la venta. Existe una tarea
-- programada que se ocupa de actualizar la página en forma periódica y para ello consulta la cotización
-- de cada uno de los 20 bancos. Cada banco dispone de una API, cuya única función es procesar las
-- solicitudes de aplicaciones externas. La tarea programada consulta de a una API por vez, esperando
-- a lo sumo 5 segundos por su respuesta. Si pasado ese tiempo no respondió, entonces se mostrará
-- vacía la información de ese banco.

Procedure BancoCentral is

    TASK Programada;

    TASK TYPE Banco is
        ENTRY ConsultaCotizacion(c: OUT Cotizaciones);
    END Banco;
    bancos = array(1..20) OF Banco;


    --- TASK BODY´s ---
    TASK BODY Banco is
        cotizaciones: Cotizaciones := asignarValores();
    BEGIN
        Accept ConsultaCotizacion(c: OUT Cotizaciones) do
            c = cotizaciones;
        END ConsultaCotizacion;
    END Banco;


    TASK BODY Programada is
        i: integer;
        arregloCotizaciones: array(1..20) OF Cotizaciones;
    BEGIN
        arregloCotizaciones := reiniciarValores();
        for i in 1 to 20 loop
            SELECT
                bancos(i).ConsultaCotizacion(arregloCotizaciones(i));
            OR DELAY (5)
                arregloCotizaciones(i) := VACIO;
            END SELECT;
        END loop;
        cargarCotizaciones(arregloCotizaciones);
    END Programada;
  

BEGIN
    null;
END BancoCentral;
