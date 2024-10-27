/** 3. Se debe modelar el funcionamiento de una casa de comida rápida, en la cual trabajan 2
cocineros y 3 vendedores, y que debe atender a C clientes. El modelado debe considerar
que:
- Cada cliente realiza un pedido y luego espera a que se lo entreguen.
- Los pedidos que hacen los clientes son tomados por cualquiera de los vendedores y se
lo pasan a los cocineros para que realicen el plato. Cuando no hay pedidos para atender,
los vendedores aprovechan para reponer un pack de bebidas de la heladera (tardan entre
1 y 3 minutos para hacer esto).
- Repetidamente cada cocinero toma un pedido pendiente dejado por los vendedores, lo
cocina y se lo entrega directamente al cliente correspondiente.
Nota: maximizar la concurrencia. **/

chan pedido(int); //Clientes realizan un pedido
chan entregaPedido(texto); //Clientes piden pedidos y Cocineros los entregan
chan vendedorLibre(int); //Vendedores avisan que están disponibles
chan atenderPedido[C](int, texto); //Coordinador avisa a Vendedores que hay pedidos
chan cocinarPedido(int, texto); //Vendedores avisan a Cocineros que hay pedidos para cocinar

Process Cliente[id:0..C-1] {
 texto pedido;

 pedido = pensarPedido();
 send(pedido(id, pedido)); //Realiza pedido
 receive(entregaPedido[id](pedido)); //Espera a que se lo entreguen
}

Process Vendedor[id:0..2] {
 int idCliente; texto pedido;

 while(true) {
   send(vendedorLibre(id)); //Avisa que está libre
   receive(atenderPedido[id](idCliente, pedido)); //Espera a que le avisen que hay un pedido
  
   if(idCliente <> -1) //Nro de cliente válido
     send(cocinarPedido(idCliente, pedido)); //Pasa pedido a los cocineros
   else
     reponerPackBebidas(delay(60..180)); //Repone pack de bebidas en un lapso que varía entre 1 y 3 minutos
 }
}

Process Coordinador {
 int idCliente, idVendedor; texto pedido;

 while(true) {
   receive(vendedorLibre(idVendedor)); //Espera a que haya un vendedor libre
   if(empty(pedido)) idCliente = -1; // Setea id inválido
   else receive(pedido(idCliente, pedido)); //Recibe el idCliente y su pedido
   send(atenderPedido[idVendedor](idCliente, pedido)); //Envía idCliente y su pedido a un vendedor
 }
}

Process Cocinero[id:0..1] {
 int idCliente; texto pedido; texto plato;

 while(true) {
   receive(cocinarPedido(idCliente, pedido)); //Espera a que haya un pedido por cocinar
   //Toma el pedido dejado por los vendedores
   plato = cocinar(pedido); //Cocina el pedido
   send(entregaPedido[idCliente], plato); //Entrega el pedido al cliente
 }
}


Nota: para cocineros no necesito que se coordine el retiro de los pedidos (a diferencia de los vendedores) porque los cocineros no tienen que hacer otra tarea si no hay pedidos pendientes a cocinar. Si tuviesen que hacer otra cosa o chequear otro canal, ahí si debería intervenir de otra forma.
