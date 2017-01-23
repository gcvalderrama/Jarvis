library(readxl)

rouge_conceptual <- read_excel("D:/Tesis2016/Jarvis/Lincoln/Rougue/rouge_v3_conceptual_rst.xlsx", 
                               sheet = "ConceptualNoExtBest")
rouge_no_expantion_conceptual <- read_excel("D:/Tesis2016/Jarvis/Lincoln/Rougue/rouge_v3_conceptual_rst.xlsx", 
                               sheet = "ConceptualRSTNoExpV3")
#View(rouge_final_conceptual)
#View(rouge_conceptual)

first_vector <- rouge_conceptual[,6]
second_vector <- rouge_no_expantion_conceptual[,6]
wilconxon = wilcox.test(first_vector, second_vector)
print(wilconxon)


print(round(mean(first_vector),3))
print(round(mean(second_vector),3))

boxplot(first_vector, second_vector)


#qqnorm(rouge1_rst_noexp)
#qqline(rouge1_rst_noexp)






