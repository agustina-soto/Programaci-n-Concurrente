/** En un examen final hay N alumnos y P profesores. Cada alumno resuelve su examen, lo entrega y
espera a que alguno de los profesores lo corrija y le indique la nota. Los profesores corrigen los
exámenes respetando el orden en que los alumnos van entregando.
  a) Considerando que P=1.
  b) Considerando que P>1.
  c) Ídem b) pero considerando que los alumnos no comienzan a realizar su examen hasta que todos hayan llegado al aula.
Nota: maximizar la concurrencia; no generar demora innecesaria; todos los procesos deben terminar su ejecución. **/

a) Considerando P = 1.

Process Alumno[id:0..N-1] {
    int id; Examen examen; Resultado res;
    Examen examen;
    examen = resolverExamen();
    Admin!examenResuelto(examen, id); //Envía al Admin el examen resuelto y su identificador
    Profesor?resultado(res); //Espera a que el profesor le envíe el resultado del examen
}

Process Profesor {
    int i, idAlumno; Examen examen; Resultado res;
    for i in 1..N {
        Admin!disponible(); //Avisa que está disponible para corregir
        Admin?examenACorregir(examen, idAlumno); //Espera a que Admin le envíe un examen a corregir
        res = corregirExamen(examen);
        Alumno[idAlumno]!resultado(res); //Envía al alumno cuyo examen corrigió la nota obtenida
    }
}

Process Admin {
    cola Buffer; Examen examen; int idAlumno, cantE = 0, totalE = N;

    do (cantE < totalE) ; Alumno[*]?examenResuelto(examen, idAlumno) -> push(Buffer(examen, idAlumno));
    [] not empty(Buffer) ; Profesor?disponible -> Profesor!examenACorregir(pop(Buffer), idAlumno); cantE++; //Si hay exámenes por corregir y el profesor está disponible, se le envía el primero al profesor
    od
}

----------------------------------------------------------------------------------------------------------------------------------

b) Considerando P > 1.

Process Alumno [id:0..N-1] {
    Examen examen;
    examen = resolverExamen();
    Admin!examenResuelto(examen, id); //Envía al Admin el examen resuelto y su identificador
    Profesor[*]?resultado(res); //Espera a que algún profesor le envíe el resultado
}

Process Profesor [id:0..P-1] {
    int idAlumno; Examen examen; boolean continuar = true;
    while(continuar) {
        Admin!disponible(id);  //Notifica disponibilidad al Admin
        Admin?examenACorregir(examen, idAlumno); //Espera a que Admin le envíe un examen a corregir
        if(idAlumno <> -1) ->
            res = corregirExamen(examen);
            Alumno[idAlumno]!resultado(res); //Envía al alumno cuyo examen corrigió la nota obtenida
        [] (idAlumno = -1) ->
            continuar = false; //No hay más exámenes para corregir
    }
}

Process Admin {
    cola Buffer; Examen examen; int i, idAlumno, idProfesor, cantE = 0, totalE = N;

    do (cantE < totalE) ; Alumno[*]?examenResuelto(examen, idAlumno) -> push(Buffer(examen, idAlumno)); //Si quedan exámenes por corregir y si algún alumno envió su examen, se agregaría al buffer el examen del alumno
    [] not empty(Buffer) ; Profesor[*]?disponible(idProfesor) -> Profesor[idProfesor]!examenACorregir(pop(Buffer), idAlumno); cantE++; //Si hay exámenes por corregir y el profesor está disponible, se le enviaría el primero en espera al profesor
    od

    // Al terminar de corregir los exámenes, envía señal de finalización a cada profesor
    for i in 0..P-1 {
        Profesor[i]!examenACorregir(null, -1); //Se les envía un idAlumno inválido
    }
}

----------------------------------------------------------------------------------------------------------------------------------

c) idem (b) pero los alumnos deben esperar a que llegue el resto para comenzar el examen.

Process Alumno [id:0..N-1] {
    Examen examen;

    Admin!llegadaAlumno(); //Avisa que llegó
    Admin?comenzarExamen(); //Espera a que el resto de los alumnos lleguen
    examen = resolverExamen();
    Admin!examenResuelto(examen, id); //Envía al Admin el examen resuelto y su identificador
    Profesor[*]?resultado(res); //Espera a que algún profesor le envíe el resultado
}

Process Profesor [id:0..P-1] {
    int idAlumno; Examen examen; boolean continuar = true;
    while(continuar) {
        Admin!disponible(id);  //Notifica disponibilidad al Admin
        Admin?examenACorregir(examen, idAlumno); //Espera a que Admin le envíe un examen a corregir
        if(idAlumno <> -1) ->
            res = corregirExamen(examen);
            Alumno[idAlumno]!resultado(res); //Envía al alumno cuyo examen corrigió la nota obtenida
        [] (idAlumno = -1) ->
            continuar = false; //No hay más exámenes para corregir
    }
}

Process Admin {
    cola Buffer; Examen examen; int i, idAlumno, idProfesor, cantE = 0, limite = N;

    //Barrera para que los alumnos esperen a que todos lleguen para comenzar con el examen
    for i in 0..N-1 --> Admin[*]?llegadaAlumno();

    //Avisa a los alumnos que pueden comenzar a resolver el examen
    for i in 0..N-1 --> Admin[i]!comenzarExamen();

    do (cantE < limite) ; Alumno[*]?examenResuelto(examen, idAlumno) -> push(Buffer(examen, idAlumno)); //Si quedan exámenes por corregir y si algún alumno envió su examen, se agregaría al buffer el examen del alumno
    [] not empty(Buffer) ; Profesor[*]?disponible(idProfesor) -> Profesor[idProfesor]!examenACorregir(pop(Buffer), idAlumno); cantE++; //Si hay exámenes por corregir y el profesor está disponible, se le enviaría el primero en espera al profesor
    od

    // Al terminar de corregir los exámenes, envía señal de finalización a cada profesor
    for i in 0..P-1 {
        Profesor[i]!examenACorregir(null, -1); //Se les envía un idAlumno inválido
    }
}

//DUDA: debería haber hecho un proceso Barrera que se encargue de los dos for (llegada y largada de los alumnos)?
