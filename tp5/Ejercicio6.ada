-- En una playa hay 5 equipos de 4 personas cada uno (en total son 20 personas donde cada
-- una conoce previamente a que equipo pertenece). Cuando las personas van llegando
-- esperan con los de su equipo hasta que el mismo esté completo (hayan llegado los 4
-- integrantes), a partir de ese momento el equipo comienza a jugar. El juego consiste en que
-- cada integrante del grupo junta 15 monedas de a una en una playa (las monedas pueden ser
-- de 1, 2 o 5 pesos) y se suman los montos de las 60 monedas conseguidas en el grupo. Al
-- finalizar cada persona debe conocer el grupo que más dinero junto. Nota: maximizar la
-- concurrencia. Suponga que para simular la búsqueda de una moneda por parte de una
-- persona existe una función Moneda() que retorna el valor de la moneda encontrada.


Procedure JuegoPlaya is

    TASK Arbitro is
        ENTRY compararTotales(id: IN integer, sumaTotal: IN integer);
        ENTRY ganador(nroEquipo: OUT integer);
    END Arbitro;

    TASK TYPE Equipo is
        ENTRY Identificador(idE: IN integer);
        ENTRY llegadaIntegrante();
        ENTRY salidaEquipo();
        ENTRY sumarTotal(sumaMontos: IN integer);
    END Equipo;

    equipos = array(1..5) OF Equipo;

    TASK TYPE Persona;

    personas = array(1..20) OF Persona;


    ---- TASK BODY´S ----
    TASK BODY Arbitro is
        maximo : integer := -1; idE: integer := -1;
    BEGIN
        for i in 1 to 5 loop
            Accept compararTotales(idE: IN integer, sumaEquipo: IN integer) do
                if (sumaEquipo > maximo)
                    maximo := sumaEquipo;
                    idMaximo := idE;
                END if;
            END compararTotales;
        END loop;

        -- Calculó el máximo, ahora hay que enviar el resultado a todos los integrantes de los 5 equipos --
        -- Debería mandarle el resultado a los equipos y que sean ellos los que les manden a los integrantes? --

        for i in 1 to 20 loop
            Accept ganador(idMaximo);
        END loop;
    END Arbitro;


    TASK BODY Equipo is
        i, id, sumaTotal: integer := 0;
    BEGIN
        Accept Identificador(idE: IN integer) do  -- Recibe quién es (su id)
            id = idE;
        END Identificador;

        for i in 1 to 4 loop
            Accept llegadaIntegrante();
        END loop;

        -- Llegaron todos los integrantes del equipo

        for i in 1 to 4 loop
            Accept salidaEquipo();
        END loop;

        -- Comienza el juego --

        for i in 1 to 4 loop
            Accept sumarTotal(unMonto: IN integer) do
                sumaTotal := sumaTotal + unMonto;
            END sumarTotal;
        END loop;

        -- Finaliza trabajo en equipo

        Arbitro.compararTotales(id, sumaTotal);  -- Envía al árbitro el total de monedas del equipo junto a su id
    END Equipo;


    TASK BODY Persona is
        nroEquipo, sumaMontos: integer;
    BEGIN
        sumaMontos := 0;
        nroEquipo = conocerEquipo();
        
        equipos(nroEquipo).llegadaIntegrante();  -- Avisa a su equipo que llegó
        equipos(nroEquipo).salidaEquipo();  -- Espera a que le permitan salir
        
        for i in 1 to 15 loop
            sumaMontos := sumaMontos + Moneda();  -- Busca una moneda, retorna el valor de la moneda encontrada y lo suma en el contador
        end loop;
        
        equipos(nroEquipo).sumarTotal(sumaMontos);  -- Avisa al equipo que tengo montos para sumar al total
        Arbitro.ganador(nroEquipoGanador);  -- Pide al arbitro que le diga cual fue el equipo ganador
        
        if(nroEquipoGanador == nroEquipo) then
            print "ganamos";
        END if;
    END Persona;


    i: integer;
BEGIN
    for i in 1 to 5 loop
        equipos(i).Identificador(i);  -- Envía a cada equipo su id
    END loop;
END JuegoPlaya;
