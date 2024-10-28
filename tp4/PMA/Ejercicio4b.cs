/** 4. Simular la atención en un locutorio con 10 cabinas telefónicas, el cual tiene un empleado
que se encarga de atender a N clientes. Al llegar, cada cliente espera hasta que el empleado
le indique a qué cabina ir, la usa y luego se dirige al empleado para pagarle. El empleado
atiende a los clientes en el orden en que hacen los pedidos. A cada cliente se le entrega un
ticket factura por la operación.

b) Modifique la solución implementada para que el empleado dé prioridad a los que
terminaron de usar la cabina sobre los que están esperando para usarla.
Nota: maximizar la concurrencia; suponga que hay una función Cobrar() llamada por el
empleado que simula que el empleado le cobra al cliente.

Nota: Los clientes que ya usaron la cabina y quieren pagar tienen prioridad ante aquellos clientes que recien llegaron y necesitan saber a qué cabina dirigirse. **/


chan llegada(int); //Clientes avisan que llegaron
chan asignarCabina[N](int); //Empleado avisa a Clientes qué cabina les asignó
chan cabinaDisponible(int); //Cliente avisa a Empleado que liberó la cabina que estaba usando
chan envioTicket[int](Ticket); //Empleado envía a Clientes el ticket del cobro
chan filaPagar(int); //Clientes avisan a Empleado que se agregan a la fila para pagar //No veo el problema en mandar el id con el canal cabinaDisponible (en vez de hacer este otro), lo dejo así para mayor representación

Process Empleado {
  int i, idCliente, nroCabina; boolean cabinasLibres[10] = {[10] true}; Ticket ticket;

  while(true) {
    //BW
    if(empty(filaPagar)) ->
      if ((not empty(llegada)) and (hayCabinaLibre(cabinasLibres))) {
        receive(llegada(idCliente)); //Espera a que llegue un cliente
        nroCabina = getRandomNroCabinaDisponible(cabinasLibres); //Obtiene un nro de cabina de las disponibles del arreglo
        cabinasLibres[nroCabina] = false; //Marca cabina como ocupada para que no se pueda volver a pedir por la funcion "getRandomNroCabinaDisponible" hasta que se libere
        send(asignarCabina[idCliente](nroCabina)); //Avisa al cliente a qué cabina tiene que ir
      }
    [] (not empty (filaPagar)) ->
        receive(cabinaDisponible(nroCabina)); // Espera a que el cliente le avise que terminó de usar la cabina
        receive(filaPagar(idCliente));
        cabinasLibres[nroCabina] = true; //Libera la cabina utilizada
        ticket = Cobrar(idCliente, nroCabina); //Se obtiene el ticket luego de cobrar al cliente
        send(envioTicket[id](ticket)); //Se envía el ticket al cliente
    end if
  }
}

Process Cliente[id:0..N-1] {
  int nroCabina; Ticket ticket;

  send(llegada(id)); //Avisa que llegó
  receive(asignarCabina[id](nroCabina)); //Espera a que el empleado le diga a qué cabina ir
  usarCabina(nroCabina);
  send(cabinaDisponible(nroCabina)); //Avisa al empleado que liberó la cabina
  send(filaPagar(id)); //Avisa que se agrega a la fila para pagar
  receive(envioTicket[id](ticket)); //Espera a que le envíen el ticket factura
}
