-- Se quiere modelar el funcionamiento de un banco, al cual llegan clientes que deben realizar
-- un pago y retirar un comprobante. Existe un único empleado en el banco, el cual atiende de
-- acuerdo con el orden de llegada.
-- a) Implemente una solución donde los clientes llegan y se retiran sólo después de haber sido
-- atendidos.
-- b) Implemente una solución donde los clientes se retiran si esperan más de 10 minutos para
-- realizar el pago.
-- c) Implemente una solución donde los clientes se retiran si no son atendidos
-- inmediatamente.
-- d) Implemente una solución donde los clientes esperan a lo sumo 10 minutos para ser
-- atendidos. Si pasado ese lapso no fueron atendidos, entonces solicitan atención una vez más
-- y se retiran si no son atendidos inmediatamente.


a) Los clientes solo se retiran despues de que son atendidos.

Procedure BancoA is

    TASK Empleado is
        ENTRY atender();
    END Empleado;

    TASK TYPE Cliente;

    arregloClientes = ARRAY(1..C) OF Cliente;


    -- TASK BODY's --

    TASK BODY Cliente is
        comprobante: texto;
    BEGIN
        Empleado.atender(comprobante); -- Espera a que lo atiendan
        -- Es atendido, paga y le dan un comprobante
    END Cliente;

    TASK BODY Empleado is
    BEGIN
        loop
            accept atender(OUT comprobante) do
                comprobante = cobrarAlCliente();
            END atender;
        END loop;
    END Empleado;

Begin
    null;
End BancoA;
