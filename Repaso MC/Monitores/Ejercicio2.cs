/* 2) Resolver el siguiente problema. En una empresa trabajan 20 vendedores ambulantes que forman 5
equipos de 4 personas cada uno (cada vendedor conoce previamente a qué equipo pertenece). Cada
equipo se encarga de vender un producto diferente. Las personas de un equipo se deben juntar antes
de comenzar a trabajar. Luego cada integrante del equipo trabaja independientemente del resto
vendiendo ejemplares del producto correspondiente. Al terminar cada integrante del grupo debe
conocer la cantidad de ejemplares vendidos por el grupo. Nota: maximizar la concurrencia. */

Process Vendedores[id:0..19] {
    int cantEjemplares = 0;  int cantTotalEjemplares;  int nroEquipo = conocerEquipo();
    Equipo[nroEquipo].comienzo();
    venderProducto(cantEjemplares);
    Equipo[nroEquipo].calcularTotal(cantEjemplares, cantTotalEjemplares);
}

Monitor Equipo[id:0..4] {
    cond espera;
    Producto producto;
    int cantIntegrantes = 0, cantTotal = 0;

    producto = conocerProductoAsignado(id);

    Procedure comienzo(prod: OUT Producto) {
        cantIntegrantes++;
        if(cantIntegrantes < 4) wait(espera);
        else signalAll(espera);
        prod = producto;  //Todos los procesos salen de este procedure sabiendo que producto tienen que vender
    }

    Procedure calcularTotal(cant: IN int, total: OUT int) {
        cantTotal += cant;
        cantIntegrantes--;  //Decrementa cantidad de integrantes (cantIntegrantes = 0 significa que ya sumo las cantidades de todo el equipo)
        if(cantIntegrantes == 0)  //Si no quedan integrantes por procesar, los desencola a todos para que puedan ver el resultado
            signalAll(espera);
        else wait(espera);  //Si quedan integrantes por procesar, encola al actual para que espere a que el resto termine
        total = cantTotal;
    }
}
