/* 3) Resolver el siguiente problema. En una montaña hay 30 escaladores que en una parte de la subida
deben utilizar un único paso de a uno a la vez y de acuerdo con el orden de llegada al mismo. Nota:
sólo se pueden utilizar procesos que representen a los escaladores; cada escalador usa sólo una vez
el paso */

Process Escalador[id:0..29] {
    Subida.llegadaPaso();
    utilizandoPaso();
    Subida.salidaPaso();
}

Monitor Subida {
    cond espera;
    int cant = 0;
    boolean libre = true;

    Procedure llegadaPaso() {
        if(!libre) {
            cant++;
            wait(espera);
        }
        else libre = false;
    }
    
    Procedure salidaPaso() {
        if(cant > 0) {
            cant--;
            signal(espera);
        }
        else libre = true;
    }
}
