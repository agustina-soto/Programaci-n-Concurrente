-- Hay un sistema de reconocimiento de huellas dactilares de la policía que tiene 8 Servidores
-- para realizar el reconocimiento, cada uno de ellos trabajando con una Base de Datos propia;
-- a su vez hay un Especialista que utiliza indefinidamente. El sistema funciona de la siguiente
-- manera: el Especialista toma una imagen de una huella (TEST) y se la envía a los servidores
-- para que cada uno de ellos le devuelva el código y el valor de similitud de la huella que más
-- se asemeja a TEST en su BD; al final del procesamiento, el especialista debe conocer el
-- código de la huella con mayor valor de similitud entre las devueltas por los 8 servidores.
-- Cuando ha terminado de procesar una huella comienza nuevamente todo el ciclo. Nota:
-- suponga que existe una función Buscar(test, código, valor) que utiliza cada Servidor donde
-- recibe como parámetro de entrada la huella test, y devuelve como parámetros de salida el
-- código y el valor de similitud de la huella más parecida a test en la BD correspondiente.
-- Maximizar la concurrencia y no generar demora innecesaria.


Procedure SistemaReconocimiento is

    TASK Especialista is
        ENTRY PedidoHuella(test: OUT Huella);
        ENTRY RecibirResultado(codigo: IN integer, valor: IN integer);
    END Especialista;

    TASK TYPE Servidor;
    servidores = array(1..8) OF Servidor;


    --- TAS BODY´s ---
    TASK BODY Especialista is
        test: Huella;
        codigoMax, valorMax: integer;
    BEGIN
        loop
            codigoMax := -1; valorMax := -1;  -- Inicializa máximos cada vez que se termina de procesar una huella
            test = tomarImagenHuella();
            for i in 1 to 16 loop  -- 16 repeticiones porque recibe 8 pedidos de huella y 8 recepciones de resultados
                SELECT
                    Accept PedidoHuella(huella: OUT Huella) do
                        huella := test;
                    END PedidoHuella;
                OR
                    Accept RecibirResultado(codigo: IN integer, valor: IN integer) do
                        if(valor > valorMax)
                            valorMax := valor;
                            codigoMax := codigo;
                        END if;
                    END RecibirResultado;
                END SELECT;
            END loop;  -- Termina de procesar una huella. Se averiguó el valor de similitud máximo.
        END loop;
    END Especialista;


    TASK BODY Servidor is
        codigo, valor: integer;
    BEGIN
        loop
            Especialista.PedidoHuella(huella);  -- Espera a que el Especialista le devuelva una huella
            Buscar(huella, codigo, valor);  -- Busca la huella recibida
            Especialista.RecibirResultado(codigo, valor);  -- Envía a Especialista el codigo y el valor de la huella enviada
        END loop;
    END Servidor;


BEGIN
    null;
END SistemaReconocimiento;
