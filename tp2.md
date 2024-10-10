# Práctica 2 - Semáforos

## Ejercicio 1
Existen N personas que deben ser chequeadas por un detector de metales antes de poder ingresar al avión.

a. Analice el problema y defina qué procesos, recursos y semáforos serán
necesarios/convenientes, además de las posibles sincronizaciones requeridas para resolver el problema.

b. Implemente una solución que modele el acceso de las personas a un detector (es decir, si el detector está libre la persona lo puede utilizar; en caso contrario, debe esperar).

c. Modifique su solución para el caso que haya tres detectores. 

### RESPUESTA:

```c
1.a)
    Procesos: Las N personas que desean pasar por el detector.
    Recursos: El detector de metales (un recurso compartido que solo puede ser utilizado por un proceso a la vez).
    Semáforo: Un semáforo binario (o de exclusión mutua) que garantiza que solo una persona acceda al detector a la vez. Se inicializa en 1.
    Sincronización: El semáforo permite que las personas esperen si el detector está ocupado y que solo una persona lo use a la vez.
```

```c
1.b)
    sem mutex_detector = 1;
    
    Process Persona [id: 0..N-1] {
        P(mutex_detector);
        // uso el detector
        V(mutex_detector);
    }
```

```c
1.c)
    sem mutex_detector = 3; // Se decrementa por si sola la funcion, cuando no hayan detectores libres queda automaticamente en 0

    Process Persona [id: 0..N-1] {
        P(mutex_detector);
        // uso el detector
        V(mutex_detector);
}
```

```c
1.d)
    sem mutex_detector = 3; // Se decrementa por si sola la funcion, cuando no hayan detectores libres queda automaticamente en 0

    Process Persona [id: 0..N-1] {
        int i, cant_chequeos = get_cant_random_int(); // Obtengo un numero entero de veces en la que es chequeada la persona [id]
        for i = 1..N {
            P(mutex_detector);
            // uso el detector
            V(mutex_detector);    
        }
    }
```


## Ejercicio 2

Un sistema de control cuenta con 4 procesos que realizan chequeos en forma
colaborativa. Para ello, reciben el historial de fallos del día anterior (por simplicidad, de tamaño N). De cada fallo, se conoce su número de identificación (ID) y su nivel de gravedad (0=bajo, 1=intermedio, 2=alto, 3=crítico). Resuelva considerando las siguientes situaciones:

**a.** Se debe imprimir en pantalla los ID de todos los errores críticos (no importa el orden).

**b.** Se debe calcular la cantidad de fallos por nivel de gravedad, debiendo quedar los resultados en un vector global.

**c.** Ídem b. pero cada proceso debe ocuparse de contar los fallos de un nivel de
gravedad determinado

### RESPUESTA:

```c
2.a)
    int N = cantidad_de_fallos_del_dia_anterior;
    colaFallos historial[N]; // Asumo que fue inicializada con los fallos del dia anterior

    sem mutex_historial = 1;

    int cantFallosProcesados = 0; // Todos los procesos van a compartir esta cantidad para no procesar fallos de más

    Process proceso [id: 0..3] {
        Fallo fallo;
        
        P(mutex_historial);
        
        while (cantFallosProcesados < N) { // Mientrras no terminen de procesarse todos los fallos
            fallo = historial.pop(); // Saco un fallo de la cola
            cantFallosProcesados++; // Incremento cantidad compartida

            V(mutex_historial); // Libero el historial, el procesamiento de la info la hago con mi variable local

            if(fallo.getNivel() == 3) {
                print("ID de fallo con nivel de gravedad 3: " + fallo.getID());
            }

            P(mutex_historial); // Para que la sincronizacion siga iterandose, para los ultimos de cada proceso se libera afuera del while
        }
        V(mutex_historial);
    }
```

```c
2.b)
    int N = cantidad_de_fallos_del_dia_anterior;
    colaFallos historial[N]; // Asumo que fue inicializada con los fallos del dia anterior

    sem mutex_historial = 1; // Semáforo para el historial de fallos (un acceso a la vez)
    sem semNivel[0..3] = [{0..3} 1]; // Semáforo para el arreglo de cantidad de fallos x nivel, a lo sumo 4 accesos a la vez (1 acceso x posicion)

    int cantFallosProcesados = 0; // Todos los procesos van a compartir esta cantidad para no procesar fallos de más

    int arregloCantFallos[0..3] = [{0..3} 0]; // Arreglo que acumula cantidad de fallos por nivel de gravedad, inicializado en 0

    Process proceso [id: 0..3] {
        Fallo fallo;
        
        P(mutex_historial);
        
        while (cantFallosProcesados < N) { // Mientrras no terminen de procesarse todos los fallos
            fallo = historial.pop(); // Saco un fallo de la cola
            cantFallosProcesados++; // Incremento cantidad compartida de fallos procesados

            V(mutex_historial); // Libero el historial, el procesamiento de la info la hago con mi variable local

            if(fallo.getNivel() == 3) {
                print("ID de fallo con nivel de gravedad 3: " + fallo.getID());
            }

            // Protego la posición que necesito del arreglo compartido para incrementar cantidad de fallos
            P(semNivel[fallo.getNivel()]);
            arregloCantFallos[fallo.getNivel()] += 1;
            V(semNivel[fallo.getNivel()]);

            P(mutex_historial); // Para que la sincronización "siga iterandose", para los últimos de cada proceso se libera afuera del while
        }
        V(mutex_historial);
    }
```

```c
2.c)
    int N = cantidad_de_fallos_del_dia_anterior;
    colaFallos historial[N]; // Asumo que fue inicializada con los fallos del dia anterior

    sem mutex_historial = 1; // Semáforo para el historial de fallos (un acceso a la vez)

    int cantFallosProcesados = 0; // Todos los procesos van a compartir esta cantidad para no procesar fallos de más

    int arregloCantFallos[0..3] = [{0..3} 0]; // Arreglo que acumula cantidad de fallos por nivel de gravedad, inicializado en 0

    Process proceso [id: 0..3] { // Cada proceso se va a encargar de procesar el nivel de gravedad "id"
        Fallo fallo;
        
        P(mutex_historial);
        
        while (cantFallosProcesados < N) { // Mientrras no terminen de procesarse todos los fallos
            fallo = historial.pop(); // Saco un fallo de la cola

            if(fallo.getNivel() == id) {
                cantFallosProcesados++;
                V(mutex_historial);
                arregloCantFallos[fallo.getNivel()] += 1; // No necesito protegerlo porque todos los procesos trabajan sobre una posición diferente
            }
            else {
                historial.push(fallo);
                V(mutex_historial);
            }

            if(fallo.getNivel() == 3) {
                print("ID de fallo con nivel de gravedad 3: " + fallo.getID());
            }

            P(mutex_historial); // Para que la sincronización "siga iterandose", para los últimos de cada proceso se libera afuera del while
        }
        V(mutex_historial);
    }
```


## Ejercicio 3

Un sistema operativo mantiene 5 instancias de un recurso almacenadas en una cola. Además, existen P procesos que necesitan usar una instancia del recurso. Para eso, deben sacar la instancia de la cola antes de usarla. Una vez usada, la instancia debe ser encolada nuevamente.

### RESPUESTA:

```c
cola c; // Asumo que está incializada con las 5 instancias
int N = 5;

// Los procesos acceden a diferentes instancias pero del mismo recurso, por lo que necesito 2 semáforos:
sem mutex_cola = 1; // Sem para el acceso a la cola
sem recursos = 5; // Sem para controlar cantidad de pops de la cola

Process proceso[id: 0..P-1] {
    instancia inst;

    while(true) {
        P(recursos); // Espero la confirmación de que hay recursos disponibles
        // Cuando haya recursos, espero a que la cola esté disponible
        P(mutex_cola);
        inst = c.pop();
        V(mutex_cola);

        // Usa la instancia

        P(mutex_cola);
        c.push(inst); // Vuelvo a encolar la instancia utilizada
        V(mutex_cola);
    }
}
```


## Ejercicio 4
Suponga que existe una BD que puede ser accedida por 6 usuarios como máximo al mismo tiempo. Además, los usuarios se clasifican como usuarios de prioridad alta y usuarios de prioridad baja. Por último, la BD tiene la siguiente restricción:
* no puede haber más de 4 usuarios con prioridad alta al mismo tiempo usando la BD.
* no puede haber más de 5 usuarios con prioridad baja al mismo tiempo usando la BD.

Indique si la solución presentada es la más adecuada. Justifique la respuesta. 

```pascal
Var
sem: semaphoro := 6;
alta: semaphoro := 4;
baja: semaphoro := 5
```
<table style="width:100%">
   <thead>
        <tr>
            <th>
                Process Usuario-Alta [I:1..L]:: 
            </th>
            <th>
                Process Usuario-Baja [I:1..K]:: 
            </th>
        </tr>
   </thead>
<tbody>
<tr>
 <td rowspan=4>

```c
 { P (sem);
 P (alta);
 //usa la BD
 V(sem);
 V(alta);
 }
```

</td>
 <td rowspan=4>

```c
{ P (sem);
 P (baja);
//usa la BD
 V(sem);
 V(baja);
 }
```
</td>
</td>
</tr>
</tbody>
</table>

### RESPUESTA:
```c
Lo que no es correcto es el orden en que están colocados los semáforos. De la manera es la que están desarrollados
los procesos no se maximiza la concurrencia y hay demora innecesaria.
Ejemplo: podría pasar que ingresen 4 usuarios de alta prioridad en forma consecutiva y se encuentren en un determinado
momento utilizando la BD. Eso no impediría que sigan pasando más usuarios de alta prioridad por el primer semáforo (P(sem)),
quedando bloqueados luego en P(alta) ya que se alcanzó el máximo de usuarios de prioridad alta. Esto es un inconveniente ya que
tranquilamente podrían haber 2 usuarios de prioridad baja utilizando al mismo tiempo la BD, pero quedarán bloqueados en el primer
semáforo. Alcanzaría simplemente con invertir el orden de los semáforos (primero liberar P(alta)/P(baja) y luego P(sem)).
Otro problema (que persiste aún invirtiendo los semáforos) es que no se maneja correctamente la prioridad de los procesos "alta"
sobre los de "baja".
```

## Ejercicio 5
En una empresa de logística de paquetes existe una sala de contenedores donde se preparan las entregas. Cada contenedor puede almacenar un paquete y la sala cuenta con capacidad para N contenedores. Resuelva considerando las siguientes situaciones:

**a)** La empresa cuenta con 2 empleados: un empleado Preparador que se ocupa de preparar los paquetes y dejarlos en los contenedores; un empelado Entregador que se ocupa de tomar los paquetes de los contenedores y realizar la entregas.
Tanto el Preparador como el Entregador trabajan de a un paquete por vez.

**b)** Modifique la solución a) para el caso en que haya P empleados Preparadores.

**c)** Modifique la solución a) para el caso en que haya E empleados Entregadores.

**d)** Modifique la solución a) para el caso en que haya P empleados Preparadores y E empleadores Entregadores.

### RESPUESTA:

// TENGO VARIOS TEMAS CON ESTE PUNTO. NO SÉ SI ESTA BIEN MANEJAR LA CONCURRENCIA ASÍ (FINGIENDO DEMENCIA ACERCA DE
 QUE NO LE MANDO A PREPARADORES NI A ENTREGADORES QUÉ RECURDO ESPECÍFICO TIENEN QUE TOMAR), O SI DEBERÍA USAR UNA 
 COLA QUE ENCOLE LOS "IDS" DE LOS CONTENEDORES, O SI DEBERÍA TENER UN ARREGLO CON EL QUE PROTEGA LOS CONTENEDORES...


```c
5.a)
sem contenedor_vacio = N;
sem contenedor_lleno = 0;

Process Preparador {
    while(true) {
        PrepararPaquete();
        P(contenedor_vacio);
        DepositarPaquete();
        V(contenedor_lleno);
    }
}

Process Entregador { 
    while(true){
        P(contenedor_lleno);
        TomarPaquete();
        V(contenedor_vacio);
        EntregarPaquete();
    }
}
´´´

```c
5.b)
sem contenedor_vacio = N;
sem contenedor_lleno = 0;
sem mutex = 1;

Process Preparador[id:0..P-1] {
    while(true) {
        PrepararPaquete();
        P(contenedor_vacio);
        P(mutex);
        DepositarPaquete();
        V(mutex);
        V(contenedor_lleno);
    }
}

Process Entregador { 
    while(true){
        P(contenedor_lleno);
        TomarPaquete();
        V(contenedor_vacio);
        EntregarPaquete();
    }
}
´´´

```c
5.c)
sem contenedor_vacio = N;
sem contenedor_lleno = 0;
sem mutex = 1;

Process Preparador {
    while(true) {
        PrepararPaquete();
        P(contenedor_vacio);
        DepositarPaquete();
        V(contenedor_lleno);
    }
}

Process Entregador[id:0..E-1] { 
    while(true){
        P(contenedor_lleno);
        P(mutex);
        TomarPaquete();
        V(mutex);
        V(contenedor_vacio);
        EntregarPaquete();
    }
}
´´´

```c
5.d)
sem contenedor_vacio = N;   // Número de contenedores vacíos, empieza en N - vi una solucion que ponia esto en 1, por que?? si hay N, tengo que poder sacar N...
sem contenedor_lleno = 0;   // Número de contenedores llenos, empieza en 0
sem mutex = 1;              // Exclusión mutua compartida por ambos, preparadores y entregadores

Process Preparador[id:0..P-1] {
    while(true) {
        PrepararPaquete();       // Prepara el paquete fuera de la zona crítica
        P(contenedor_vacio);     // Espera hasta que haya un contenedor vacío
        P(mutex);                // Exclusión mutua para acceder a los contenedores
        DepositarPaquete();      // Deposita el paquete en un contenedor
        V(mutex);                // Libera la exclusión mutua
        V(contenedor_lleno);     // Señala que hay un contenedor lleno
    }
}

Process Entregador[id: 0..E-1] { 
    while(true){
        P(contenedor_lleno);     // Espera hasta que haya un contenedor lleno
        P(mutex);                // Exclusión mutua para acceder a los contenedores
        TomarPaquete();          // Toma un paquete del contenedor
        V(mutex);                // Libera la exclusión mutua
        V(contenedor_vacio);     // Señala que hay un contenedor vacío
        EntregarPaquete();       // Entrega el paquete fuera de la zona crítica
    }
}
```


## Ejercicio 6
Existen N personas que deben imprimir un trabajo cada una. Resolver cada ítem usando semáforos:

**a)** Implemente una solución suponiendo que existe una única impresora compartida por todas las personas, y las mismas la deben usar de a una persona a la vez, sin importar el orden. Existe una función `Imprimir(documento)` llamada por la persona que simula el uso de la impresora. Sólo se deben usar los procesos que representan a las Personas.

**b)** Modifique la solución de a) para el caso en que se deba respetar el orden de llegada.

**c)** Modifique la solución de (a) para el caso en que se deba respetar estrictamente el orden dado por el identificador del proceso (la persona X no puede usar la impresora hasta que no haya terminado de usarla la persona X-1).

**d)** Modifique la solución de (b) para el caso en que además hay un proceso Coordinador que le indica a cada persona que es su turno de usar la impresora.

**e)** Modificar la solución (d) para el caso en que sean 5 impresoras. El coordinador le indica a la persona cuando puede usar una impresora, y cual debe usar. 

### RESPUESTA:

```c
6.a)
sem mutex_impresora = 1; // Las personas usan una misma impresora, de a una persona a la vez

Process Persona[id:0..N-1] {
    Documento documento;
    P(mutex_impresora); // Espero siempre que mutex_impresora es 0, sino avanzo a la siguiente instrucción (como inicializo en 1 porque se usa de a una persona a la vez, el semáforo va de 1 a 0 y de 0 a 1)
    Imprimir(documento);
    V(mutex_impresora);
}
```

```c
6.b)
cola colaImpresora;
boolean libre = true; // Estado de la impresora
sem mutex = 1;
sem espera [N] = ([N] 0); // Necesito que las personas esperen a que se les otorgue su turno

Process Persona[id:0..N-1] {
    int siguiente;
    Documento documento;

    P(mutex);
    if(not libre) { // Si la impresora no está libre
        colaImpresora.push(id);
        V(mutex);
        P(espera[id]); // La persona "id" se queda esperando a que le toque su turno
    }
    else {
        libre = false; // Indica que la impresora está ocupada
        V(mutex);
    }

    // Se llega de a 1 proceso a la vez a esta línea --> sé que el que llega es al que le toca imprimir
    Imprimir(documento);

    P(mutex);
    if(empty(colaImpresora))
        libre = true; // Indica impresora libre
    else { // Toma el siguiente
        siguiente = colaImpresora.pop();
        V(espera[siguiente]);
    }
    V(mutex);
}
```

```c
6.c)
sem espera_turno[N] = ([N] 0); // Las personas van a esperar hasta que el proceso "id-1" los despierte
int turno = 0;

Process Persona[id:0..N-1] {
    Documento documento;
    if(id != turno) {
        P(espera_turno[id]);
    }
    Imprimir(documento); // No necesito proteger la impresora, la espera x los turnos permite que si o si lleguen hasta acá de a 1 proceso
    turno++; // Incrementa el turno para habilitar al proceso con el siguiente id
    V(espera_turno[turno]);
}
```

```c
6.d)
cola colaImpresora;
sem mutex_cola = 1;
sem espera_turno [N] = ([N] 0); // Necesito que las personas esperen a que se les otorgue su turno
sem llegue = 0, fin = 1;

Process Persona[id:0..N-1] {
    Documento documento;

    // Se encola en la fila de personas que quieren imprimir
    P(mutex_cola);
    colaImpresora.push(id);
    V(mutex_cola);

    V(llegue); // Avisa al coordinador que está esperando en la cola

    P(espera_turno[id]); // Espera a que el coordinador le diga que es su turno

    Imprimir(documento);

    V(fin); // Avisa al coordinador que terminó de usar el recurso
}

Process Coordinador {
    int siguiente;

    for i := 0 to N-1 {
        P(llegue); // Espera a que alguien le avise que está esperando en la cola
        
        P(mutex_cola);
        siguiente = colaImpresora.pop(); // Recupera el id del proceso que sigue
        V(mutex_cola);

        V(espera_turno[siguiente]); // Despierta al proceso al que le toca su turno
        P(fin); // Espera a que el proceso termine de usar la impresora
    }
}
```

```c
6.e)
cola colaIdProcesos, colaImpresoras;
int id_impresoras[N];
sem mutex_cola = 1, mutex_impresoras = 1;
sem espera_turno[N] = ([N] 0);              // Necesito que las personas esperen a que se les otorgue su turno
sem llegue = 0, impresoras_libres = 5;      // ESTO ESTÁ MAL. CORREGIR!!

Process Persona[id:0..N-1] {
    Documento documento;

                                            // Se encola en la fila de personas que quieren imprimir
    P(mutex_cola);
    colaIdProcesos.push(id);
    V(mutex_cola);

    V(llegue);                              // Avisa al coordinador que está esperando en la cola

    P(espera_turno[id]);                    // Espera a que el coordinador le diga que es su turno

    int id_impresora = id_impresoras[id];   // Averigua en que impresora tiene que imprimir
    Imprimir(documento, id_impresora);      // La función Imprimir se encarga de que el documento se imprima en la impresora del id enviado

    P(mutex_impresoras);
    colaImpresoras.push(id_impresora);      // Avisa que terminó de usar la impresora
    V(mutex_impresoras);

    V(impresoras_libres);                   // Avisa al coordinador que terminó de usar el recurso - Incrementa cant de impresoras disponibles
}


Process Coordinador {
    int i, siguiente, idImp;

    for i := 0 to 4 {                       // Pushea todos los ids de impresoras, las impresoras de la cola son las disponibles
        colaImpresoras.push(i);
    }

    for i := 0 to N-1 {
        P(llegue);                          // Espera a que alguien le avise que está esperando en la cola

        P(mutex_cola);
        siguiente = colaIdProcesos.pop();   // Recupera el id del proceso que sigue
        V(mutex_cola);

        P(impresoras_libres);               // Antes de bloquear la cola de impresoras, espera la confirmación de que hay alguna libre
        P(mutex_impresoras);                // Recupera el id de impresora que le toca al siguiente proceso
        idImp = colaImpresoras.pop();
        V(mutex_impresoras);

        id_impresoras[siguiente] = idImp;   // Carga el id de la impresora a utilizar en la posición del proceso al que le toca

        V(espera_turno[siguiente]);         // Despierta al proceso al que le toca su turno
    }
}
```

## Ejercicio 7
Suponga que se tiene un curso con 50 alumnos. Cada alumno debe realizar una tarea y existen 10 enunciados posibles. Una vez que todos los alumnos eligieron su tarea, comienzan a realizarla. Cada vez que un alumno termina su tarea, le avisa al profesor y se queda esperando el puntaje del grupo, el cual está dado por todos aquellos que comparten el mismo enunciado. Cuando un grupo terminó, el profesor les otorga un puntaje que representa el orden en que se terminó esa tarea de las 10 posibles.

_**Nota:**_ Para elegir la tarea suponga que existe una función `elegir` que le asigna una tarea a un alumno (esta función asignará 10 tareas diferentes entre 50 alumnos, es decir, que 5 alumnos tendrán la tarea 1, otros 5 la tarea 2 y así sucesivamente para las 10 tareas).

### RESPUESTA:

```c
int contador = 0;
int puntajeGrupo[10] = ([10] 0);
cola finalizadas; // Cola con las tareas finalizadas

sem mutex = 1; // Restringe acceso a la cola; reutilizado para restringir el incremento del contador y el acceso a la elección de enunciados
sem barrera_eleccion = 0, tarea_finalizada = 0, espera_puntaje[10] = ({10} 0);


Process Alumno[id:0..49] {
    int i, nro_tarea, puntaje;

    P(mutex); // Protego uso del contador y de la función elegir()

    nro_tarea = elegir(); // Función que elige una tarea
    contador++; // Incrementa cantidad de alumnos que eligieron un enunciado
    if(contador == 50) { // Si todos los alumnos eligieron un enunciado
        for i := 0 to 49 {
            V(barrera_eleccion}); // Despierta a todos los alumnos
        }
    }

    V(mutex); // Libera los recursos compartidos

    P(barrera_eleccion); // Espera a que todos los alumnos eligan un enunciado

    RealizarTarea(nro_tarea); // Empieza la tarea

    P(mutex);
    finalizadas.push(nro_tarea); // Apila el nro de tarea que acaba de realizarse
    V(mutex);

    V(tarea_finalizada); // Avisa al profesor que terminó su tarea

    P(espera_puntaje[nro_tarea]); // Espera el puntaje grupal
    puntaje = puntajeGrupo[nro_tarea]; // Obtiene el puntaje en base al orden en el que terminó su grupo
}

Process Profesor {
    int i, j, nro_tarea_finalizada, orden = 0; // Orden en el que terminan los grupos (de 1 a 10)
    contadorTareas[N] = ({N} 0);

    for i := 1 to 50 {
        P(tarea_finalizada);

        P(mutex);
        nro_tarea_finalizada = finalizadas.pop(); // Desapila el nro de tarea de la que terminó primero de las que se encuentran en la cola
        V(mutex); // Todo el procesamiento está dentro del mismo semáforo porque sino es posible que la variable nro_tarea_finalizada sea sobreescrita

	contadorTareas[nro_tarea_finalizada]++; // Incrementa la cantidad de tareas terminadas del nro de tarea "nro_tarea_finalizada"
        if(contadorTareas[nro_tarea_finalizada] == 5) { // Inmediatamente después debe chequear si ya terminaron todos los del grupo
            puntajeGrupo[++orden];
            for j := 1 to 5 {
                V(espera_puntaje[nro_tarea_finalizada]); // Despierta a los alumnos que realizaron la tarea "nro_tarea_finalizada"
            }
        }
    }
}
```

## Ejercicio 8
Una fábrica de piezas metálicas debe producir T piezas por día. Para eso, cuenta con E empleados que se ocupan de producir las piezas de a una por vez (se asume T>E). La
fábrica empieza a producir una vez que todos los empleados llegaron. Mientras haya piezas por fabricar, los empleados tomarán una y la realizarán. Cada empleado puede tardar distinto tiempo en fabricar una pieza. Al finalizar el día, se le da un premio al empleado que más piezas fabricó.

### RESPUESTA:

```c
sem mutex_piezas = 1; // Acceso a cantidad de piezas producidas
sem mutex_contador = 1; // Acceso a contador para la barrera de empleados

sem espera_empleados = 0;

sem despertar_empresa = 0;

int piezas_por_empleado[E] = ({E} 0); // Acumula cantidad de piezas producidas por empleado
int cont_barrera = 0, total_piezas_tomadas = 0, int id_empleado_maximo = -1;


Process Empleado[id:0..E-1] {
    Pieza p;
    boolean gane = false;

    // Llega un empleado

    P(mutex_contador);
    cont_barrera++; // Incrementa la cantidad de empleados que llegaron
    if(cont_barrera == E) {
        for i = 1 to E -> V(espera_empleados);
    }
    V(mutex_contador);
    
    P(espera_empleados);

    P(mutex_piezas);
    while(total_piezas_tomadas < T) { // Mientras hay piezas por fabricar
        TomarPieza(p);
        total_piezas_tomadas++; // Incrementa en 1 total piezas tomadas
        V(mutex_piezas);

        RealizarPieza(p);

        // Incrementa en 1 su cantidad de piezas producidas
        piezas_por_empleado[id]++;
    }

    V(despertar_empresa); // Avisa a la empresa que terminó de fabricar
    P(espera_empleados); // Espera a que le digan qué empleado es el que más piezas fabricó

    if(id_empleado_maximo == id) {
        gane = true;
    }
}


Process Empresa {
    int i;

    for i = 1 to 50 {
        P(despertar_empresa); // Espera a que todos los empleados terminan de fabricar
    }

    id_empleado_maximo = CalcularMaximo(piezas_por_empleado); // Función que devuelve el id de empleado que más piezas fabricó

    for i = 1 to 50 {
        V(espera_empleados);
    }
}
```

## Ejercicio 9
Resolver el funcionamiento en una fábrica de ventanas con 7 empleados (4 carpinteros, 1 vidriero y 2 armadores) que trabajan de la siguiente manera:
* Los carpinteros continuamente hacen marcos (cada marco es armando por un único carpintero) y los deja en un depósito con capacidad de almacenar 30 marcos.

* El vidriero continuamente hace vidrios y los deja en otro depósito con capacidad para 50 vidrios.

* Los armadores continuamente toman un marco y un vidrio (en ese orden) de los depósitos correspondientes y arman la ventana (cada ventana es armada por un único armador).

### RESPUESTA:

```c
sem mutex_deposito_vidrios = 1;
sem mutex_deposito_marcos = 1;
sem mutex_deposito_ventanas = 1;

sem espacios_disponibles_vidrios = 50;
sem espacios_disponibles_marcos = 30;

sem hay_vidrios = 0;
sem hay_marcos = 0;

cola depositoVidrios;
cola depositoMarcos;
cola depositoVentanas;

Process Vidriero {
    Vidrio v;

    while(true) {
        v = RealizarVidrio();

        P(espacios_disponibles_vidrios); // Si el dep de vidrios está habilitado, pasa y decrementa un cupo; si no hay lugar, queda esperando

        P(mutex_deposito_vidrio);
        cola_vidrios.push(v);
        V(mutex_deposito_vidrio);

        V(hay_vidrios); // Incrementa la cantidad disponible de vidrios: pusheo uno, incremento el semaforo como si fuese un contador
    }
}

Process Carpintero[id:0..3] {
    Marco m;

    while(true) {
        m = RealizarMarco();

        P(espacios_disponibles_marcos); // Si el dep de marcos está habilitado, pasa y decrementa un cupo; si no hay lugar, queda esperando

        P(mutex_deposito_marco);
        cola_marcos.push(m);
        V(mutex_deposito_marco);

        V(hay_marcos); // Incrementa la cantidad disponible de marcos: pusheo uno, incremento el semaforo como si fuese un contador
    }
}

Process Armador[id:0..1] {
    Marco m; Vidrio v; Ventana ventana;

    while(true) {
        P(hay_marcos); // Si hay marcos en el deposito, decremento la cantidad del semáforo; sino espero a que haya
        
        P(mutex_deposito_marcos);
        m = cola_marcos.pop();
        V(mutex_deposito_marcos);

        V(espacios_disponibles_marcos); // Incremento el cupo de capacidad de marcos (para que se deposite otro)

        P(hay_vidrios); // Si hay vidrios en el deposito, decremento la cantidad del semáforo; sino espero a que haya

        P(mutex_deposito_vidrios);
        v = cola_vidrios.pop();
        V(mutex_deposito_vidrios);

        V(espacios_disponibles_vidrios); // Incremento el cupo de capacidad de vidrios (avisa que hay espacio para que se deposite otro)

        ventana = ArmarVentana(m, v); // Arma la ventana con el marco y el vidrio tomado

        P(mutex_deposito_ventanas);
        cola_ventanas.push(ventana);
        V(mutex_deposito_ventanas);
    }
}
```

## Ejercicio 10
A una cerealera van T camiones a descargar trigo y M camiones a descargar maíz. Sólo
hay lugar para que 7 camiones a la vez descarguen, pero no pueden ser más de 5 del mismo
tipo de cereal.

**a)** Implemente una solución que use un proceso extra que actúe como coordinador
entre los camiones. El coordinador debe atender a los camiones según el orden de
llegada. Además, debe retirarse cuando todos los camiones han descargado.

**b)** Implemente una solución que no use procesos adicionales (sólo camiones). No
importa el orden de llegada para descargar. Nota: maximice la concurrencia.

### RESPUESTA:

```c
10.b)
sem mutex = 1, lugar_maiz = 5, lugar_trigo = 5, lugares = 7; 

int cantLugares = 0;
Process CamionTrigo[id:0..T-1]
{
  while(true){
    P(lugar_trigo); // Espera a que haya lugar para un camion de trigo

    P(lugares); // Espera a que haya lugar para un camion

    descargar();

    V(lugares); // Libera lugar para un camion

    V(lugar_trigo); // Libera lugar para un camion de trigo
  }

}

Process CamionMaiz[id:0..M-1]
{
  while (true){
    P(lugar_maiz); // Espera a que haya lugar para un camion de maiz

    P(lugares); // Espera a que haya lugar para un camion

    descargar();

    V(lugares); // Libera lugar para un camion

    V(lugar_maiz); // Libera lugar para un camion de maiz
  }
}
```

```c
10.a) LO EMPECÉ PERO NO LO TERMINÉ (ni cerca)
sem capacidad_camiones = 7, lugar_trigo = 5, lugar_maiz = 5, llego_camion = 0;
sem mutex_cola = 1;

Process CamionTrigo[id:0..T-1] {
    // Llega un camion
    
    P(mutex_cola);
    cola_trigo.push(id);
    V(mutex_cola);

    V(llego_camion);

    P(lugar_trigo);
    P(capacidad_camiones);

    DescargarTrigo();

    V(capacidad_camiones);
    V(lugar_trigo);
}

Process CamionMaiz[id:0..M-1] {
    // Llega un camion
    
    P(mutex_cola);
    cola_maiz.push(id);
    V(mutex_cola);

    V(llego_camion);

    P(lugar_maiz);
    P(capacidad_camiones);

    DescargarMaiz();

    V(capacidad_camiones);
    V(lugar_maiz);
}

Process Coordinador {
    Camion c;
    while(true) {
        P(llego_camion); // Espera a que haya un camion por atender

        c = cola_
         // Le avisa al camion que esta primero que puede descargar su contenido
        // Espera a que el camion le confirme que terminó de hacer su trabajo
    }
    // Se retira cuando todos los camiones han descargado
}
```


## Ejercicio 11
En un vacunatorio hay **un empleado** de salud para vacunar a **50 personas**. El empleado de salud atiende a las personas de acuerdo con el orden de llegada y de a 5 personas a la
vez. Es decir, que cuando está libre debe esperar a que haya al menos 5 personas esperando, luego vacuna a las 5 primeras personas, y al terminar las deja ir para esperar por otras 5. Cuando ha atendido a las 50 personas el empleado de salud se retira. 

_**Nota:**_ todos los procesos deben terminar su ejecución; asegurarse de no realizar _Busy Waiting_; suponga que el empleado tienen una función `VacunarPersona()` que simula que el empleado está vacunando a UNA persona. 

### RESPUESTA:

```c
cola c;
int cant_personas_vacunadas = 0, cant_personas_esperando = 0;

sem mutex = 1;
sem barrera_personas = 5;
sem espera_vacunarse[50] = ({0..49} 0);

Process Empleado {
    int i, j, id_persona;
    cola vacunados;

    for i = 1 to 10 {
        P(despertar_empleado); // Espera a que le avisen que hay 5 personas esperando

        for j = 1 to 5 {
            P(mutex);
            c.pop(id_persona);
            V(mutex);

            VacunarPersona(id_persona); // Vacuna a la persona con id_persona
            vacunados.push(id_persona); // Agrega a la persona vacunada a la cola de vacunados para liberar a todos despues
        }

        // Termina de vacunar a las 5 personas

        for j = 1 to 5 {    // Espera a terminar con las 5 personas para dejarlas ir a todas juntas
            id_persona = vacunados.pop();
            V(espera_vacunarse[id_persona]);
        }
    }

    // Se retira
}

Process Persona[id:0..49] {
    // Llega una persona

    P(mutex); // --- Pide recursos compartidos
    c.push(id);
    cant_personas_esperando++;
    if(cant_personas_esperando == 5) {
        V(despertar_empleado);
        cant_personas_esperando = 0; // Reinicia contador
    }
    V(mutex);  // --- Libera recursos compartidos
    
    P(espera_vacunarse[id]); // Queda a la espera de que la vacunen, así le permiten irse

    // Se retira
}
```

## Ejercicio 12
Simular la atención en una Terminal de Micros que posee 3 puestos para hisopar a **150 pasajeros**. En cada puesto hay **una Enfermera** que atiende a los pasajeros de acuerdo con el orden de llegada al mismo. Cuando llega un pasajero se dirige al Recepcionista, quien le indica qué puesto es el que tiene menos gente esperando. Luego se dirige al puesto y espera a que la enfermera correspondiente lo llame para hisoparlo. Finalmente, se retira.

**a)** Implemente una solución considerando los procesos Pasajeros, Enfermera y
Recepcionista.

**b)** Modifique la solución anterior para que sólo haya procesos Pasajeros y Enfermera,
siendo los pasajeros quienes determinan por su cuenta qué puesto tiene menos
personas esperando.

_Nota:_ suponga que existe una función Hisopar() que simula la atención del pasajero por
parte de la enfermera correspondiente.


### RESPUESTA:

_// ASUMO QUE EXISTE LA FUNCIÓN "PuestoConMenosPasajeros()" QUE DEVUELVE EL PUESTO (LA COLA) QUE TIENE MENOS PASAJEROS ESPERANDO_


```c
12.a)
int cantP = 0;
cola recepcion, puesto[0..2];
sem mutex_recep = 1, mutex_hisopar[0..2] = ({3} 1);
sem espera_recepcion = 0, asignar_puesto[0..149] = ({150} 0); espera_hisopado[0..149] = ({150} 0), avisar_enfermera[0..149] = ({150} 0);
int puestos_pasajeros[0..149] = ({150} -1); // Nro de puesto asignado a cada pasajero

Process Pasajero[id:0..149] {
    // Llega un pasajero

    P(mutex_recep);
    recepcion.push(id); // Se agrega a la cola de recepción
    V(mutex_recep);

    V(espera_recepcion); // Avisa al recepcionista que llegó

    P(asignar_puesto[id]); // Recepción le dice a qué puesto ir

    V(avisar_enfermera[puestos_pasajeros[id]]); // Despierta a la enfermera asignada al pasajero en el array de puestos
    
    P(espera_hisopado[id]); // Espera a que la enfermera lo hisope en el puesto
    
    // Se retira
}

Process Recepcionista {
    Puesto nro_puesto; int i;

    for i = 1 to 150 {
        P(espera_recepcion); // Espera a que llegue un pasajero

        P(mutex_recep);
        id = cola_recep.pop(); // Toma al primer pasajero que se encoló
        V(mutex_recep);

        nro_puesto = PuestoConMenosPasajeros();

        P(mutex_hisopar[nro_puesto]);
        puesto[nro_puesto].push(id); // Agrego al final de la cola a un nuevo pasajero
        V(mutex_hisopar[nro_puesto]);

        puesto_pasajeros[id] = nro_puesto; // Asigna puesto "nro_puesto" al pasajero "id"

        V(asignar_puesto[id]); // El pasajero se va
    }
}

Process Enfermera[id:0..2] {
    Pasajero pas;

    P(mutex_contador);
    while(cantP < 150) {

        P(avisar_enfermera[id]); // Espera a que llegue un pasajero

        P(mutex_hisopar[id]); // Espera a que se libere el acceso a la cola
        pas = puesto[id].pop(); // Saca de su puesto (puesto nro "id") el primer pasajero en la cola
        V(mutex_hisopar[id]);
        
        Hisopar(); // Lo hisopa
        
        cantP++; // Incrementa cantidad de pasajeros hisopados totales

        V(espera_hisopado[pas]); // Avisa al pasajero "pas" que terminó de hisoparlo

        V(mutex_contador);

        P(mutex_contador);
    }
}
```

```c
12.b)
int cantP = 0;
cola cola_espera[3];
sem mutex = 1, mutex_cola[3] = ([3] 1), mutex_hisopar = 1;
sem espera_persona[150] = ([150] 0), ocupada[3] = ([3] 0);

Process Pasajero[id:0..149]{
    int puesto;
    P(mutex);
    puesto = PuestoConMenosPasajeros();
    V(mutex);
    P(mutex_cola[cola])
    cola_espera[cola].push(id);
    V(mutex_cola[cola])
    V(ocupada[cola]);

    P(espera_persona[id]);
    // Está siendo hisopado
    P(espera_persona[id]);
}

Process Enfermera[id:0..2]{
    int pasajero;
    int j;
    while(cantP < 150){
        P(ocupada[i]);
        if (!cola_espera[id].isEmpty()){
            P(mutex_cola[id]);
            pasajero = cola_espera[id].pop();
            V(mutex_cola[id]);
            V(espera_persona[pasajero]);
            Hisopar(pasajero);
            V(espera_persona[pasajero]);
            P(mutex_hisopar);
            cantP++;
            if (cantP == 150){
                for j = 0..2 -> V(ocupada[j]);
            }
            V(mutex_hisopar);
        }
    }
}
```
