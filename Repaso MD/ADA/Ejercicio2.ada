-- Resolver el siguiente problema. En un negocio de cobros digitales hay P personas que deben pasar
-- por la única caja de cobros para realizar el pago de sus boletas. Las personas son atendidas de acuerdo
-- con el orden de llegada, teniendo prioridad aquellos que deben pagar menos de 5 boletas de los que
-- pagan más. Adicionalmente, las personas ancianas tienen prioridad sobre los dos casos anteriores.
-- Las personas entregan sus boletas al cajero y el dinero de pago; el cajero les devuelve el vuelto y los
-- recibos de pago.


Procedure Cobro is

    TASK Cajero is
        ENTRY CobrarAnciana(boletas: IN Boletas, dinero: IN double,recibos: OUT Recibos, vuelto: OUT double);
        ENTRY CobrarMenosBoletas(boletas: IN Boletas, dinero: IN double,recibos: OUT Recibos, vuelto: OUT double);
        ENTRY CobrarComun(boletas: IN Boletas, dinero: IN double,recibos: OUT Recibos, vuelto: OUT double);
    END Cajero;

    TASK TYPE Persona;
    personas = array(1..P) OF Persona;

    --- TASK BODY´s ---
    TASK BODY Persona is
        boletas: Boletas;
        recibos: Recibos;
        dinero, vuelto: double;
    BEGIN
        boletas := AsignarBoletas();
        dinero := AsignarDinero();
        if(es anciana)
            Cajero.CobrarAnciana(boletas, dinero, recibos, vuelto);
        else if (boletas.size() < 5)
            Cajero.CobrarMenosBoletas(boletas, dinero, recibos, vuelto);
        else
            Cajero.CobrarComun(boletas, dinero, recibos, vuelto);
        END if;
    END Persona;


    TASK BODY Cajero is
        i: integer;
    BEGIN
        for i in 1 to P loop
            SELECT
                Accept CobrarAnciana(boletas: IN Boletas, dinero: IN double,recibos: OUT Recibos, vuelto: OUT double) do
                    Recibos, vuelto := procesarPago(boletas, dinero);
                END CobrarAnciana;
            OR
                WHEN(CobrarAnciana´count = 0)
                    Accept CobrarMenosBoletas(boletas: IN Boletas, dinero: IN double,recibos: OUT Recibos, vuelto: OUT double) do
                        Recibos, vuelto := procesarPago(boletas, dinero);
                    END CobrarMenosBoletas;
            OR
                WHEN(CobrarAnciana´count = 0) AND (CobrarMenosBoletas´count = 0)  -- Sólo va a entrar cuando no haya personas ancianas y cuando no haya personas con menos de 5 boletas
                    Accept CobrarComun() do
                        Recibos, vuelto := procesarPago(boletas, dinero);
                    END CobrarComun;
            END SELECT;
        END loop;
    END Cajero;


BEGIN
    null;
END Cobro;
