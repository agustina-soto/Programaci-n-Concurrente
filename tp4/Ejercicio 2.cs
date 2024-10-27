/** 2. Se desea modelar el funcionamiento de un banco en el cual existen 5 cajas para realizar
pagos. Existen P clientes que desean hacer un pago. Para esto, cada una selecciona la caja
donde hay menos personas esperando; una vez seleccionada, espera a ser atendido. En cada
caja, los clientes son atendidos por orden de llegada por los cajeros. Luego del pago, se les
entrega un comprobante. Nota: maximizar la concurrencia. **/

chan llegada[5](int);
chan esperaTurno[P](int);
chan pagar[5](Pago);
chan entregaComprobante[P](texto);

Process Cliente[id:0..P-1] {
 int nroCaja; texto comprobante; Pago pago;
 
 nroCaja = getCajaMenosClientes(); // Obtengo caja con menos clientes
 send(llegada[nroCaja](id)); // Aviso a la caja que estoy en su cola
 receive(esperaTurno[id]()); // Me quedo bloqueado hasta que sea mi turno

 //Soy atendido, realizo el pago
 pago = realizarPago();

 send(pagar[nroCaja](pago)); //Aviso que ya pagué
 receive(entregaComprobante[id](comprobante)); //Espero el comprobante
}

Process Cajero[id:0..4] {
 int idCliente; Pago pago; Comprobante comprobante;

 // Espera a que llegue un cliente
 receive(llegada[id](idCliente));

 // Aviso al cliente que es su turno
 send(esperaTurno[idCliente](1));

 // Atiendo al cliente
 
 // Espera a que el clinete pague
 receive(pagar[id](pago));
 
 comprobante = generarComprobante(pago);

 // Envia comprobante al cliente
 send(entregaComprobante[idCliente](comprobante));
}
