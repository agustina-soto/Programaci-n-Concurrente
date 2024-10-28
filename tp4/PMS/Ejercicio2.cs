/** En un laboratorio de genética veterinaria hay 3 empleados. El primero de ellos continuamente prepara
las muestras de ADN; cada vez que termina, se la envía al segundo empleado y vuelve a su trabajo.
El segundo empleado toma cada muestra de ADN preparada, arma el set de análisis que se deben realizar
con ella y espera el resultado para archivarlo. Por último, el tercer empleado se encarga de realizar
el análisis y devolverle el resultado al segundo empleado. **/

Process PrimerEmpleado {
    Muestra muestra;
    while(true) {
        muestra = prepararMuestra();
        Admin!muestraDisponible(muestra); //Envía muestra al segundo empleado
    }
}

Process SegundoEmpleado {
    SetAnalisis set; Muestra muestra;
    while(true) {
        Admin!disponible(); //Envía al Admin que está disponible
        Admin?muestraParaAnalizar(muestra); //Recibe una muestra para armar su set de análisis
        set = armarSetDeAnalisis(muestra);
        TercerEmpleado!analisisListo(set); //Envía set de análisis al tercer empleado
        TercerEmpleado?resultadoAnalisis(res); //Espera a que el tercer empleado envíe los resultados de análisis
        archivarResultado(res);
    }
}

Process TercerEmpleado {
    Set set; Resultado res;
    SegundoEmpleado?analisisListo(set);
    res = realizarAnalisis(setAnalisis);
    SegundoEmpleado!resultadoAnalisis(res); //Envía resultado al segundo empleado
}

Process Admin {
    Set set; Resultado res; cola Buffer;

    do PrimerEmpleado?muestraDisponible(muestra) -> push(Buffer, muestra);
    [] not empty(Buffer); SegundoEmpleado?disponible() -> SegundoEmpleado!muestraParaAnalizar(pop(Buffer));
    od
}

/* Es necesario el administrador para que el primer empleado haga su tarea lo mas rapido posible.
La primera vez Buffer está vacía asique si o si le va a pedir al primer empleado que devuelva
una muestra. A partir de la segunda vuelta, puede seleccionar pedirle otra muestra al 1er empleado
o pedirle al 2do empleado que arme el set. */
