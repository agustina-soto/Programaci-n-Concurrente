En una oficina existen 100 empleados que envían documentos para imprimir en 5 impresoras
compartidas. Los pedidos de impresión son procesados por orden de llegada y se asignan a la primera
impresora que se encuentre libre:

chan envioDoc(int, texto);
chan copiaImpresa(texto);

process empleado [id:0..99] {
    while true {
        doc = generarDoc();
        send envioDoc(id, doc);
        receive copiaImpresa[id](copia);
    }
}

process impresora [id:0..4] {
    while true {
        receive envioDoc(idE, doc);
        copia = imprimir(doc);
        send copiaImpresa[idC](copia);
    }
}
