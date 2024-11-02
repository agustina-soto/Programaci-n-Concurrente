-- Se debe calcular el valor promedio de un vector de 1 millón de números enteros que se
-- encuentra distribuido entre 10 procesos Worker (es decir, cada Worker tiene un vector de
-- 100 mil números). Para ello, existe un Coordinador que determina el momento en que se
-- debe realizar el cálculo de este promedio y que, además, se queda con el resultado. Nota:
-- maximizar la concurrencia; este cálculo se hace una sola vez.


Procedure Promedio is

    TASK Coordinador is
        ENTRY Comenzar();
        ENTRY sumaParcial(suma: IN integer);
    END Coordinador;

    TASK TYPE Worker;
    workers = array(1..10) OF Worker;

    --- TASK BODY´s ---
    TASK BODY Coordinador is
        sumaTotal: integer := 0;
    BEGIN
        -- Son 10 workers y cada uno hace 2 llamados --
        for i in 1 to 20 loop
            SELECT
                Accept Comenzar();
            OR
                Accept sumaParcial(suma: IN integer) do
                    sumaTotal := sumaTotal + suma;
                END sumaParcial;
            END SELECT;
        END loop;

        promedio = sumaTotal / 1000000
    END Coordinador;


    TASK BODY Worker is
        i, suma: integer := 0;
        arregloNumeros: array(1..100000) OF integer;
    BEGIN
        arregloNumeros := inicializarArregloNumeros();

        Coordinador.Comenzar();  -- Espera a que el coordinador le diga que puede empezar
        for i in 1 to 100000 loop
            suma := suma + arregloNumeros(i);
        END loop;
        Coordinador.sumaParcial(suma);
    END Worker;


BEGIN
    null;
END Promedio;
