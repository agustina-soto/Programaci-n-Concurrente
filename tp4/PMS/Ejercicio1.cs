/** 1. Suponga que existe un antivirus distribuido que se compone de R procesos robots
Examinadores y 1 proceso Analizador. Los procesos Examinadores están buscando
continuamente posibles sitios web infectados; cada vez que encuentran uno avisan la
dirección y luego continúan buscando. El proceso Analizador se encarga de hacer todas las
pruebas necesarias con cada uno de los sitios encontrados por los robots para determinar si
están o no infectados.
a) Analice el problema y defina qué procesos, recursos y comunicaciones serán
necesarios/convenientes para resolverlo.
b) Implemente una solución con PMS sin tener en cuenta el orden de los pedidos.
c) Modifique el inciso (b) para que el Analizador resuelva los pedidos en el orden
en que se hicieron. **/

a,b,c) Lo resolví teniendo en cuenta el orden de los pedidos.
Process Examinador[id:0..R-1] {
    texto sitioWeb;
    while(true) {
        sitioWeb = buscarVirus();
        Buffer!sitio(sitioWeb); //Envía sitio posiblemente infectado
}

Process Analizador {
    texto sitioWeb;
    while(true) {
        Buffer!avisoDisponible(); //Avisa que está disponible para recibir un pedido
        Buffer?sitio(sitioWeb); //Recibe un sitio a verificar
        analizarSitio(sitioWeb); //Verifica si está infectado
    }
}

Process Buffer {
    texto sitioWeb; cola Buffer;
    do Examinador[*]?sitio(sitioWeb) -> push (Buffer, sitioWeb); //Recibe un sitio y lo agrega a la fila de sitios a analizar
    [] not empty(Buffer); Analizador?avisoDisponible() -> Analizador!hacerPrueba(pop (Buffer)); //Si el Buffer no está vacío, espera a que el Analizador avise que está listo para recibir un pedido. Sólo si el buffer no está vacío (el examinador ha hecho un pedido y fue cargado en el buffer) y si el analizador está listo para recibir un pedido nuevo, se toma un pedido del buffer y se lo envía al Analizador
    od
    }
}
