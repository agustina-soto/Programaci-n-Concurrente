-- 3. Se dispone de un sistema compuesto por 1 central y 2 procesos periféricos, que se
-- comunican continuamente. Se requiere modelar su funcionamiento considerando las
-- siguientes condiciones:
-- * La central siempre comienza su ejecución tomando una señal del proceso 1; luego
    -- toma aleatoriamente señales de cualquiera de los dos indefinidamente. Al recibir una
    -- señal de proceso 2, recibe señales del mismo proceso durante 3 minutos.
-- * Los procesos periféricos envían señales continuamente a la central. La señal del
    -- proceso 1 será considerada vieja (se deshecha) si en 2 minutos no fue recibida. Si la
    -- señal del proceso 2 no puede ser recibida inmediatamente, entonces espera 1 minuto y
    -- vuelve a mandarla (no se deshecha).

Procedure SistemaComputo

    TASK Central is
        ENTRY signal1(s: IN Signal);
        ENTRY signal2(s: IN Signal);
        ENTRY finTimer();
    END Central;

    TASK Proceso1;
    TASK Proceso2;

    TASK Timer is
        ENTRY iniciar(seg: IN integer);
    END Timer;

    -- TASK BODY's --
    TASK BODY Timer is
        seg: integer;
        loop
            accept iniciar(seg) do
                DELAY(seg);  -- Espera de tantos segundos como se hayan recibido
            END iniciar;
            Central.finTimer();  -- Avisa al Central que finalizó el temporizador
        END loop;
    END Timer;


    TASK BODY Central is
        accept signal1(s: IN Signal);  -- Espera la senial del proceso 1 para dar inicio al sistema
        loop
            SELECT
                accept signal1(s: IN Signal) do
                    procesarSignal(s);
                END signal1;
            OR
                accept signal2(s: IN signal) do
                    procesarSignal(s);
                    Timer.iniciar(180);  -- Inicia un timer de 3 minutos
                    while (finTimer'count > 0) loop  -- Mientras finTimer no tenga mensajes (si tuviese significaría que le avisaron que finalizó el timer)
                        accept signal2(s: IN signal) do
                            procesarSignal(s);
                        END signal2;
                    END loop;
                    accept finTimer();  -- Recibe la finalizacion del timer (es importante desencolar para que siga funcionando!!)
                END signal2;
            END SELECT;
        END loop;
    END Central;


    TASK BODY Proceso1 is
        Signal s;
        loop
            s = generarSignal();
            SELECT
                Central.signal1(s);
            OR DELAY (120)  -- Si no se acepta la signal en 2 minutos, es desecha
                null;
            END SELECT;
        END loop;
    END Proceso1;


    TASK BODY Proceso2 is
        Signal s; boolean generarNuevaSignal = true;
        loop
            if (generarNuevaSignal) s = generarSignal();
            SELECT
                Central.signal2(s);
                generarNuevaSignal = true;  -- Se acepto la signal, por lo que es necesario generar otra en la proxima vuelta del loop
            ELSE  -- Si no se acepta inmediatamente, espera 1 minuto y vuelve a mandar la senial
                DELAY(60);  -- Espera un minuto y vuelve al loop para mandar la senial
                generarNuevaSignal = false;  -- No se debe generar una nueva senial porque hay una vieja por recibir
            END SELECT;
        END loop;
    END Proceso2;


BEGIN
    null;
END SistemaComputo;
