-- Se requiere modelar un puente de un único sentido que soporta hasta 5 unidades de peso.
El peso de los vehículos depende del tipo: cada auto pesa 1 unidad, cada camioneta pesa 2
unidades y cada camión 3 unidades. Suponga que hay una cantidad innumerable de
vehículos (A autos, B camionetas y C camiones). Analice el problema y defina qué tareas,
recursos y sincronizaciones serán necesarios/convenientes para resolverlo.
a. Realice la solución suponiendo que todos los vehículos tienen la misma prioridad.
b. Modifique la solución para que tengan mayor prioridad los camiones que el resto de los
vehículos. --



-- a. Todos los vehículos tienen la misma prioridad. --

Procedure Puente is

    TASK Admin is
        ENTRY entrarAuto();
        ENTRY salirAuto();
        ENTRY entrarCamioneta();
        ENTRY salirCamioneta();
        ENTRY entrarCamion();
        ENTRY salirCamion();
    END Admin;

    TASK TYPE Auto;
    TASK TYPE Camioneta;
    TASK TYPE Camion;

    arregloAutos = ARRAY(1..A) OF Auto;
    arregloCamionetas = ARRAY(1..B) OF Camioneta;
    arregloCamiones = ARRAY(1..C) OF Camion;

    TASK BODY Auto is
        Admin.entrarAuto();
        pasarPuente();
        Admin.salirAuto();
    END Auto;

    TASK BODY Camioneta is
        Admin.entrarCamioneta();
        pasarPuente();
        Admin.salirCamioneta();
    END Camioneta;

    TASK BODY Camion is
        Admin.entrarCamion();
        pasarPuente();
        Admin.salirCamion();
    END Camion;

    TASK BODY Admin is
        contador: integer := 0;  -- Inicia con el puente libre (0 unidades de peso)
    BEGIN
        loop
            SELECT
            ---- Entradas ----
                WHEN(contador <= 4) =>  accept entrarAuto();       -- Si hay al menos una unidad libre, permite el paso del auto
                                        contador++;                -- Incrementa 1 unidad para un auto
            OR
                WHEN(contador <= 3) =>  accept entrarCamioneta();  -- Si hay al menos dos unidades libres, permite el paso de la camioneta
                                        contador := contador + 2;  -- Incrementa 2 unidades para una camioneta
            OR
                WHEN(contador <= 2) =>  accept entrarCamion();     -- Si hay al menos tres unidades libres, permite el paso del camion
                                        contador := contador + 3;  -- Incrementa 3 unidades para un camion
            OR
            ---- Salidas ----
                accept salirAuto();
                contador--;  -- Libera 1 unidad para un auto
            OR
                accept salirCamioneta();
                contador := contador -2;  -- Libera 2 unidades para una camioneta
            OR
                accept salirCamion();
                contador := contador -3;  -- Libera 3 unidades para un camion
            END SELECT
        END loop;
    END Admin;

BEGIN
    null;
END Puente;



-- b. Los camiones tienen mayor tienen mayor prioridad que el resto de los vehículos. --
