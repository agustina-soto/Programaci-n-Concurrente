-- 5. En un sistema para acreditar carreras universitarias, hay UN Servidor que atiende pedidos
-- de U Usuarios de a uno a la vez y de acuerdo con el orden en que se hacen los pedidos.
-- Cada usuario trabaja en el documento a presentar, y luego lo envía al servidor; espera la
-- respuesta de este que le indica si está todo bien o hay algún error. Mientras haya algún error,
-- vuelve a trabajar con el documento y a enviarlo al servidor. Cuando el servidor le responde
-- que está todo bien, el usuario se retira. Cuando un usuario envía un pedido espera a lo sumo
-- 2 minutos a que sea recibido por el servidor, pasado ese tiempo espera un minuto y vuelve a
-- intentarlo (usando el mismo documento).


Procedure SIU is

    TASK Servidor is
        ENTRY Pedido(doc: IN texto, ok: OUT boolean);
    END Servidor;

    TASK TYPE Usuario;
    usuarios = array(1..U) OF Usuario;


    ---- TASK BODY´s ----
    TASK BODY Servidor is
        cant_ok: integer := 0;  -- Lleva la cantidad de documentos aceptados
    BEGIN
        while(cant_ok < U) loop
            Accept Pedido(doc: IN texto, ok: OUT boolean) do
                if (verificarDoc(doc))  -- ok va a tomar el resultado de la verificacion del documento (true - aceptado, false - denegado)
                    ok := true;
                    cant_ok++;
                else
                    ok := false;
                END if;
            END Pedido;
        END loop;
    END Servidor;


    TASK BODY Usuario is
        doc_pendiente: boolean := true;
        doc: texto;
    BEGIN
        doc = generarDocumento();
        while(doc_pendiente) loop
            SELECT
                Servidor.Pedido(doc, ok);  -- Envía pedido al servidor y espera a que le diga si está ok o si hay que modificarlo
                if(not ok)
                    doc = corregirDoc();
                else
                    doc_pendiente := false;  -- Actualiza el flag para que deje de iterar, ya se envió correctamente el documento
                END if;
            OR DELAY (120)   -- Espera hasta 2 minutos para que el servidor acepte el pedido
                DELAY(60);  -- Espera 1 minuto antes de reenviar
            END SELECT;
        END loop;
    END Usuario;

BEGIN
    null;
END SIU;
