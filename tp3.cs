/* 1) Se dispone de un puente por el cual puede pasar un solo auto a la vez. Un auto pide permiso
para pasar por el puente, cruza por el mismo y luego sigue su camino.
  a. ¿El código funciona correctamente? Justifique su respuesta.
  b. ¿Se podría simplificar el programa? ¿Sin monitor? ¿Menos procedimientos? ¿Sin variable condition? En caso afirmativo, rescriba el código.
  c. ¿La solución original respeta el orden de llegada de los vehículos? Si rescribió el código en el punto b), ¿esa solución respeta el orden de llegada? */

  
a. La solución es parcialmente correcta; cuando un auto va a pasar, llama al procedure entrarPuente() y le envia como argumento "a", parametro no declarado en el Monitor (por esto es parcialmente correcto).
  Alli va a incrementar la variable cant e iterar mientras esa variable sea mayor a 0, para que el resto de los autos vayan a esperar dormidos y recién cuando el auto que esta cruzando llame a salirPuente(),
  disminuir la variable cant (va a despertar a un auto que está esperando para cruzar y este pasara a competir por el monitor nuevamente).

b. Si, se puede simplificar haciendo que el monitor represente unicamente el cruce del puente. Esto se debe a que solo se necesita exclusion mutua, una caracteristicaque proveen de por si los monitores.

Monitor Puente {
    Procedure cruzarPuente () {
        cruzandoPuente();
    }
}

Process Auto [a:0..M-] {
    Puente.cruzarPuente();
}

c. No, no se respeta el orden de llegada ya que cuando un auto se despierta, pasa a competir con el resto de autos para pasar al puente sin ningun tipo de prioridad.
  El inciso b) tampoco lo respeta ya que todos los autos van a competir por el uso del monitor.

------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

/* 2) Existen N procesos que deben leer información de una base de datos, la cual es administrada
por un motor que admite una cantidad limitada de consultas simultáneas.
a) Analice el problema y defina qué procesos, recursos y monitores/sincronizaciones
serán necesarios/convenientes para resolverlo.
b) Implemente el acceso a la base por parte de los procesos, sabiendo que el motor de
base de datos puede atender a lo sumo 5 consultas de lectura simultáneas. */

Monitor Motor {
    int cant = 0;
    cond espera;

    Procedure llegada() {
        while(cant == 5) wait(espera);
        cant++;
    }

    Procedure salida() {
        cant--;
        if(cant > 0) signal(espera);
    }
}

Process Lectores [id:0..N-1] {
    Motor.llegada();
    LeerInformacionBD();
    Motor.salida();
}

------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

/* Existen N personas que deben fotocopiar un documento. La fotocopiadora sólo puede ser usada por una persona a la vez. Analice el problema y defina qué procesos, recursos y
monitores serán necesarios/convenientes, además de las posibles sincronizaciones requeridas para resolver el problema. Luego, resuelva considerando las siguientes situaciones:
a) Implemente una solución suponiendo no importa el orden de uso. Existe una función Fotocopiar() que simula el uso de la fotocopiadora.
b) Modifique la solución de (a) para el caso en que se deba respetar el orden de llegada.
c) Modifique la solución de (b) para el caso en que se deba dar prioridad de acuerdo con la edad de cada persona (cuando la fotocopiadora está libre la debe usar la persona de mayor
edad entre las que estén esperando para usarla).
d) Modifique la solución de (a) para el caso en que se deba respetar estrictamente el orden dado por el identificador del proceso (la persona X no puede usar la fotocopiadora
hasta que no haya terminado de usarla la persona X-1).
e) Modifique la solución de (b) para el caso en que además haya un Empleado que le indica a cada persona cuando debe usar la fotocopiadora.
f) Modificar la solución (e) para el caso en que sean 10 fotocopiadoras. El empleado le indica a la persona cuál fotocopiadora usar y cuándo hacerlo. */

a)
Monitor Fotocopiadora {
    Procedure usar(doc IN Documento; copia OUT Documento) {
        copia = Fotocopiar(doc);
    }
}

Process Persona[id:0..N-1] {
    Documento copia; Documento doc = generarDoc();
    Fotocopiadora.usar(doc, copia);
}


b)
Monitor Fotocopiadora {
    cond espera;
    int cant = 0;
    boolean libre = true;
    
    Procedure llegada() {
        if(!libre) {
            cant++;
            wait(espera);
        }
        else libre = false;
    }

    Procedure salida() {
        if(cant > 0) {
            cant--;
            signal(espera);
        }
        else libre = true;
    }
}

Process Persona[id:0..N-1] {
    Documento copia; Documento doc = generarDoc();
    Fotocopiadora.llegada();
    copia = Fotocopiar(doc);
    Fotocopiadora.salida();
}


c)
Monitor Fotocopiadora {
    cond espera[N];
    ColaOrdenada cola;
    boolean libre = true;
    
    Procedure llegada(idP: IN int, edad: IN int) {
        if(!libre) {
            insertOrd(cola(idP, edad)); //Agrega de forma ordenada por edad en la cola
            wait(espera[idP]);  //Duerme el proceso agregado
        }
        else libre = false;  //Actualiza estado de fotocopiadora
    }

    Procedure salida() {
        if(!cola.isEmpty()) {  //Si hay alguien esperando
            idP = cola.pop();  //Toma el id de la primer persona encolada
            signal(espera[idP]);  //Despierta a la persona desencolada
        }
        else libre = true;  //Actualiza estado de fotocopiadora
    }
}

Process Persona[id:0..N-1] {
    Documento copia; Documento doc = generarDoc(); int edad = getEdad();
    Fotocopiadora.llegada(id, edad);
    copia = Fotocopiar(doc);
    Fotocopiadora.salida();
}


d)
Monitor Fotocopiadora {
    int proximo = 0;
    cond espera[N];

    Procedure llegada(idP: IN int) {
        if(idP != proximo) wait(espera[idP]);
    }

    Procedure salida() {
        proximo++;
        signal(espera[proximo]);
    }
}

Process Persona[id:0..N-1] {
    Documento copia; Documento doc = generarDoc();
    Fotocopiadora.llegada(id);
    copia = Fotocopiar(doc);
    Fotocopiadora.salida();
}


e) // No se si se cumple el orden de llegada asi...
Monitor Fotocopiadora {
    cond empleado; cond persona; cond fin;
    int cant = 0;
    
    Procedure llegada() {
        signal(empleado);  //Despierta al empleado para avisarle que hay alguien esperado atencion
        cant++;  //Incrementa variable contadora
        wait(persona);  //Se duerme al proceso que llamo al procedure mientras espera a que le digan su turno
    }

    Procedure salida() {
        signal(fin);  //Despierta al empleado para avisarle que termino de usar la fotocopiadora
    }

    Procedure Administrar() {
        if(cant == 0) wait(empleado);  //Si no hay nadie esperando atencion, el empleado se duerme
        cant--;  //Decrementa cantidad de atenciones pendientes requeridas
        signal(persona);  //Despierta a la persona que pidio atencion
        wait(fin);  //Espera a que la persona termine para cederle la fotocopiadora a otra persona sin que se rompa la exclusion mutua
    }
}

Process Persona[id:0..N-1] {
    Documento copia; Documento doc = generarDoc();
    Fotocopiadora.llegada();
    copia = Fotocopiar(doc);
    Fotocopiadora.salida();
}

Process Empleado {
    for int i in 1 to N {
        Fotocopiadora.Administrar();
    }
}


f) 
Monitor Fotocopiadora {
    Cola cola, cola_fotocopiadoras = {0, 1, 2, 3, 4, 5, 6, 7, 8, 9};
    cond empleado, persona[N], fotocopiadora_libre;
    int asignada[n] = asignarFotocopiadoras();
    
    Procedure llegada(idP: IN int, fotocopiadora_asignada OUT int) {
        cola.push(idP);
        signal(empleado);  //Despierta al empleado para avisarle que hay alguien esperado atencion
        wait(persona[idP]);  //Se duerme al proceso que llamo al procedure mientras espera a que le digan que es su turno
        fotocopiadora_asignada = asignada[idP];
    }

    Procedure salida(fotocopiadora_asignada: IN int) {
        cola_fotocopiadoras.push(fotocopiadora_asignada); //Encolar la fotocopiadora que termino de usar
        signal(fotocopiadora_libre);  //Avisa que termino de usar una fotocopiadora (hay una fotocopiadora libre)
    }

    Procedure Administrar() {
        if(cola.isEmpty()) {
            wait(empleado);  //Si no hay nadie esperando, el empleado se duerme
        }
        //Hay alguien esperando atencion
        idAux = cola.pop();  //Toma el id de una persona
        if(cola_fotocopiadoras.isEmpty()) {
            wait(fotocopiadora_libre);  //Si no hay fotocopiadoras disponibles, se encola el proceso que llamo al procedimiento
        }
        //Hay fotocopiadoras libres
        asignada[idAux] = cola_fotocopiadoras.pop();  //Desencola una fotocopidora libre
        signal(persona[idAux]); //Despierta a la persona que lamo al procedimiento
    }
}

Process Persona[id:0..N-1] {
    Documento copia, doc = generarDoc(); int nro_fotocopiadora;
    Fotocopiadora.llegada(id, nro_fotocopiadora);
    copia = nro_fotocopiadora.Fotocopiar(doc);
    Fotocopiadora.salida(nro_fotocopiadora);
}

Process Empleado {
    for int i in 1 to N {
        Fotocopiadora.Administrar();
    }
}

------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

/* 4) Existen N vehículos que deben pasar por un puente de acuerdo con el orden de llegada.
Considere que el puente no soporta más de 50000kg y que cada vehículo cuenta con su propio
peso (ningún vehículo supera el peso soportado por el puente). */

Process Vehiculo[id:0..N-1] {
    real peso = getPeso();
    Puente.entrada(peso);
    cruzandoPuente();
    Puente.salida(peso);
}

Monitor Puente {
    cond espera, pesoExcedido;
    int pesoAcumulado = 0, esperando = 0;
    boolean libre = true;

    Procedure entrada(peso: IN real) {
        if(!libre) {
            esperando++;
            wait(espera);
        }
        else libre = false;

        //Hay alguien queriendo pasar
        
        /* Si se supera el peso maximo, se duerme al proceso para que el/los otros procesos que esten transitando el puente se vayan
        y permitan mas peso sobre el mismo (porque se sabe que ningun vehiculo supera el peso soportado por el puente) */
        while(pesoAcumulado + peso > 50000) {
            wait(pesoExcedido);
        }

        //Puede pasar
        pesoAcumulado += peso;
        if(esperando > 0) {  //Si hay alguien esperando, decrementa la cantidad y despierta al primero de la cola
            esperando--;
            signal(espera);
        }
        /* Si no hay nadie esperando, actualiza el estado de libre para que el proximo vehiculo que intente entrar pase directamente 
        a consultar si le da el peso para entrar ya */
        else libre = true;
    }

    Procedure salida(p: IN real) {
        pesoAcumulado -= p;  //Decremento el peso acumulado que estaria transitando el puente
        signal(pesoExcedido);  //Despierta (avisa) al primer proceso encolado para que rechequee si le da el peso para ingresar
    }
}













