-- Una empresa de limpieza se encarga de recolectar residuos en una ciudad por medio de 3
-- camiones. Hay P personas que hacen reclamos continuamente hasta que uno de los camiones
-- pase por su casa. Cada persona hace un reclamo y espera a lo sumo 15 minutos a que llegue
-- un camión; si no pasa, vuelve a hacer el reclamo y a esperar a lo sumo 15 minutos a que
-- llegue un camión; y así sucesivamente hasta que el camión llegue y recolecte los residuos.
-- Sólo cuando un camión llega, es cuando deja de hacer reclamos y se retira. Cuando un camión
-- está libre la empresa lo envía a la casa de la persona que más reclamos ha hecho sin ser atendido.
-- Nota: maximizar la concurrencia.


Procedure LimpiezaResiduos;

    TASK Admin is
        ENTRY CamionDisponible(idP: OUT integer);
        ENTRY Reclamar(idP: IN integer);
    END Admin;

    TASK TYPE Camion;
    camiones = array(1..3) OF Camion;

    TASK TYPE Persona is
        ENTRY Identificador(id: IN integer);
        ENTRY Recoleccion();
    END Persona;
    personas = array(1..P) OF Persona;

    --- TASK BODY´s ---
    TASK BODY Persona is
        id: integer;
        reclamo_sin_respuesta: boolean := true;
    BEGIN
        Accept Identificador(idP: IN integer) do  -- Recibe su identificador
            id := idP;
        END Identificador;

        while(reclamo_sin_respuesta) loop
            Admin.Reclamar(id);
            SELECT
                Accept Recoleccion();
                reclamo_sin_respuesta := false;  -- Actualiza flag, el reclamo fue respondido
            OR DELAY (900);  -- Espera hasta 15 minutos
                null;
            END SELECT;
        END loop;
    END Persona;


    TASK BODY Camion is
        idP: integer;
    BEGIN
        loop
            Admin.CamionDisponible(idP);  -- Pide al admin un id de persona cuya casa debe visitar
            personas(idP).Recoleccion();  -- Va a recolectar los residuos de la persona indicada
        END loop;
    END Camion;


    TASK BODY Admin is
        id, totalReclamos: integer;
        arregloContadorReclamos = array(1..P) OF integer;  -- Arreglo que llevará la cantidad de reclamos hechos x cada persona sin ser atendida
    BEGIN
        totalReclamos := 0; arregloContadorReclamos := inicializarArregloContador();
        loop
            SELECT
                Accept Reclamar(idP: IN integer) do  --Acepta el reclamo de una persona
                    id := idP;
                END Reclamar;
                arregloContadorReclamos(id)++; totalReclamos++;
            OR
                WHEN(totalReclamos > 0) =>  -- Sólo entra cuando hay reclamos pendientes por camiones
                    Accept CamionDisponible(idP: OUT integer) do
                        idP := obtenerMaximo(arregloContadorReclamos);
                        arregloContadorReclamos(idP) := 0;  -- Reinicializa el contador de reclamos de la persona idP
                END CamionDisponible;
            END SELECT;
        END loop;
    END Admin;

    i: integer;
BEGIN
    for i in 1 to P loop
        personas(i).Identificador(i);
    END loop;
END LimpiezaResiduos;
