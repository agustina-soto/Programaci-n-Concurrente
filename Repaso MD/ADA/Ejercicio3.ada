--Resolver el siguiente problema. La oficina central de una empresa de venta de indumentaria debe
-- calcular cuántas veces fue vendido cada uno de los artículos de su catálogo. La empresa se compone
-- de 100 sucursales y cada una de ellas maneja su propia base de datos de ventas. La oficina central
-- cuenta con una herramienta que funciona de la siguiente manera: ante la consulta realizada para un
-- artículo determinado, la herramienta envía el identificador del artículo a las sucursales, para que cada
-- una calcule cuántas veces fue vendido en ella. Al final del procesamiento, la herramienta debe
-- conocer cuántas veces fue vendido en total, considerando todas las sucursales. Cuando ha terminado
-- de procesar un artículo comienza con el siguiente (suponga que la herramienta tiene una función
-- generarArtículo() que retorna el siguiente ID a consultar). Nota: maximizar la concurrencia. Existe
-- una función ObtenerVentas(ID) que retorna la cantidad de veces que fue vendido el artículo con
-- identificador ID en la base de la sucursal que la llama.

Procedure EmpresaIndumentaria is

    TASK Herramienta is
        ENTRY Siguiente(id: OUT integer);
        ENTRY Resultado(res: IN integer);
    END Herramienta;

    TASK TYPE Sucursal;
    sucursales = array(1..100) OF Sucursal;

    
    --- TASK BODY´s ---
    TASK BODY Sucursal is
        id, cant: integer := 0;
    BEGIN
        loop
            Herramienta.Siguiente(id);  -- Pide id a buscar
            cant := ObtenerVentas(id);  -- Busca el id en su bd
            Herramienta.Resultado(cant);  -- Envía el resultado
        END loop;
    END Sucursal;


    TASK BODY Herramienta is
        i, idABuscar, result: integer
        total: integer := 0;
    BEGIN
        loop
            idABuscar := generarArtículo();

            for i in 1 to 200 loop
                SELECT
                    Accept Siguiente(id: OUT integer) do
                        id := idABuscar;
                    END Siguiente;
                OR
                    Accept Resultado(res: OUT integer) do   -- Debería haber contado sobre total directamente no? al pedo estoy usando "result"
                        result := res;
                    END Resultado;
                    total := total + result;
                END SELECT;
            END loop;

            print("Cantidad: " + total + "para articulo con ID " + idABuscar);
        
        END loop;
    END Herramienta;


BEGIN
    null;
END EmpresaIndumentaria;
