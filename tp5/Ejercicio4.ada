-- 4. En una clínica existe un médico de guardia que recibe continuamente peticiones de
-- atención de las E enfermeras que trabajan en su piso y de las P personas que llegan a la
-- clínica ser atendidos.
-- Cuando una persona necesita que la atiendan espera a lo sumo 5 minutos a que el médico lo
-- haga, si pasado ese tiempo no lo hace, espera 10 minutos y vuelve a requerir la atención del
-- médico. Si no es atendida tres veces, se enoja y se retira de la clínica.
-- Cuando una enfermera requiere la atención del médico, si este no lo atiende inmediatamente
-- le hace una nota y se la deja en el consultorio para que esta resuelva su pedido en el
-- momento que pueda (el pedido puede ser que el médico le firme algún papel). Cuando la
-- petición ha sido recibida por el médico o la nota ha sido dejada en el escritorio, continúa
-- trabajando y haciendo más peticiones.
-- El médico atiende los pedidos dándole prioridad a los enfermos que llegan para ser atendidos.
-- Cuando atiende un pedido, recibe la solicitud y la procesa durante un cierto tiempo. Cuando
-- está libre aprovecha a procesar las notas dejadas por las enfermeras.


Procedure Clinica is

    TASK Medico is
        ENTRY AtencionEnfermera(peticion: IN texto);
        ENTRY AtencionPersona(sintomas: IN texto);
    END Medico;

    TASK Consultorio is
        ENTRY dejarNota(nota: IN texto);
        ENTRY agarrarNota(nota: OUT texto);
    END Consultorio;

    TASK TYPE Enfermera;
    enfermeras = ARRAY(1..E) OF Enfermera;

    TASK TYPE Persona;
    personas = ARRAY(1..P) OF Persona;

    
    -- TASK BODY's --
    TASK BODY Medico is
        nota: texto;
    BEGIN
        loop
            SELECT
                Accept AtencionPersona(sintomas: IN texto) do
                    atenderPersona(sintomas);
                END AtencionPersona;
            OR
                WHEN (AtencionPersona'count = 0)  =>    -- Solo lo va a hacer si no hay personas esperando atencion
                                                        Accept AtencionEnfermera(peticion: IN texto) do
                                                            procesarPeticion(peticion);
                                                        END AtencionEnfermera;
            ELSE  -- Si no hay pacientes esperando ni enfermeras que requieran atencion, se procesa una nota dejada en el consultorio (si la hay).
                SELECT  -- Si hay una nota, la toma y resuelve la peticion
                    Consultorio.agarrarNota(nota);  -- Intenta agarrar una nota (si no la hay, no entra al select)
                    resolverPeticion(nota);
                ELSE
                    null;
                END SELECT;
            END SELECT;
        END loop;
    END Medico;


    TASK BODY Consultorio is
        notas: colaDeTexto;
    BEGIN
        loop
            SELECT
                Accept dejarNota(nota: IN texto) do  -- Acepta que una enfermera deje una nota
                    notas.push(nota);
                END dejarNota;
            OR
                Accept agarrarNota(nota: OUT texto) do  -- Acepta que el medico tome una nota de la cola
                    nota = pop(notas);
                END agarrarNota;
            END SELECT;
        END loop;
    END Consultorio;


    TASK BODY Enfermera is
        peticion, nota: texto;
    BEGIN
        loop
            peticion = trabajar();
            SELECT
                Medico.AtencionEnfermera(peticion);  -- Pide atencion al medico
            ELSE  -- Si el medico no le acepta instantaneamente, le escribe una nota y se la deja en el consultorio
                nota = generarNota();
                Consultorio.dejarNota(nota);
            END SELECT;
        END loop;
    END Enfermera;


    TASK BODY Persona is
        sintomas: texto;
        intentos: integer := 0;
        atencion_pendiente: boolean := true;
    BEGIN
        sintomas = averiguarSintomas();
        while ( (atencion_pendiente)  AND  (intentos < 3) ) loop
            SELECT
                Medico.AtencionPersona(sintomas);
                atencion_pendiente = false;  -- Actualiza estado de atencion
            OR DELAY (300)  -- Espera a lo sumo 5 minutos a que el medico lo atienda
                intentos++;  -- Incrementa cantidad de intentos fallidos
                DELAY(600);  -- Espera 10 minutos y vuelve a requerir la atencion
            END SELECT;
        END loop;
    END Persona;

BEGIN
    null;
END Clinica;
