/** 4. Simular la atención en un locutorio con 10 cabinas telefónicas, el cual tiene un empleado
que se encarga de atender a N clientes. Al llegar, cada cliente espera hasta que el empleado
le indique a qué cabina ir, la usa y luego se dirige al empleado para pagarle. El empleado
atiende a los clientes en el orden en que hacen los pedidos. A cada cliente se le entrega un
ticket factura por la operación.
a) Implemente una solución para el problema descrito (sin prioridades). **/


chan llegada(int); //Clientes avisan que llegaron
chan asignarCabina[C](int); //Empleado avisa a Clientes qué cabina les asignó
chan liberarCabina; //Cliente avisa a Empleado que liberó la cabina que estaba usando
chan envioTicket[int](Ticket); //Empleado envía a Cliente el ticket del cobro

Process Empleado {
 int i, idCliente, nroCabina; boolean cabinasLibres[10] = {[10] true}; Ticket ticket;

 while(true) {
   receive(llegada(idCliente)); //Espera a que llegue un cliente

   nroCabina = getRandomNroCabinaDisponible(cabinasLibres); //Obtiene un nro de cabina de las disponibles del arreglo

   cabinasLibres[nroCabina] = false; //Marca cabina como ocupada para que no se pueda volver a pedir por la funcion "getRandomNroCabinaDisponible" hasta que se libere

   send(asignarCabina[idCliente](nroCabina)); //Avisa al cliente a qué cabina tiene que ir
   receive(liberarCabina()); // Espera a que el cliente le avise que terminó de usar la cabina
   cabinasLibres[nroCabina] = true; //Libera la cabina utilizada
   ticket = Cobrar(idCliente, nroCabina); //Se obtiene el ticket luego de cobrar al cliente
   send(envioTicket[id](ticket)); //Se envía el ticket al cliente
   }
}

Process Cliente[id:0..N-1] {
 int nroCabina; Ticket ticket;

 send(llegada(id)); //Avisa que llegó
 receive(asignarCabina[id](nroCabina)); //Espera a que el empleado le diga a qué cabina ir
 usarCabina(nroCabina);
 send(liberarCabina()); //Avisa al empleado que liberó la cabina
 receive(envioTicket[id](ticket)); //Espera a que le envíen el ticket factura
}
